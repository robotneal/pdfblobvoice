using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using InvoiceToPdf.Invoices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;

namespace InvoiceToPdf.Storage;

public interface IInvoiceStore
{
    Invoices.InvoiceModel GetInvoice(string invoiceNumber);
    void SaveInvoice(InvoiceModel invoice);
}

internal class InvoiceStore : IInvoiceStore
{
    private readonly ILogger<InvoiceStore> _logger;
    private readonly string _location;
    private readonly Credentials _cred;
    private readonly IInvoiceCrypto _invoiceCrypto;

    public InvoiceStore(ILogger<InvoiceStore> logger, IConfiguration config, IInvoiceCrypto invoiceCrypto, Credentials cred)
    {
        _logger = logger;

        _location = config["StorageLocation"];
        _cred = cred;
        _invoiceCrypto = invoiceCrypto;
    }

    public Invoices.InvoiceModel GetInvoice(string invoiceNumber)
    {
        var result = GetClient(invoiceNumber).DownloadContent();
        return System.Text.Json.JsonSerializer.Deserialize<InvoiceModel>(Encoding.UTF8.GetString(result.Value.Content.ToArray()));
    }

    public void SaveInvoice(InvoiceModel invoice)
    {
        var jsonData = System.Text.Json.JsonSerializer.Serialize(invoice);
        GetClient(invoice.InvoiceNumber).Upload(new BinaryData(Encoding.UTF8.GetBytes(jsonData)));
    }

    private BlobClient GetClient(string invoiceNumber) =>
        new (
            new Uri(_location + $"/{invoiceNumber}"),
            _cred.Get,
            new SpecializedBlobClientOptions
            {
                ClientSideEncryption = _invoiceCrypto.GetBlobEncryptionOptions()
            });
}
