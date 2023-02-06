using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using Azure.Storage;
using InvoiceToPdf.Invoices;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace InvoiceToPdf.Storage;

public interface IInvoiceCrypto
{
    InvoiceModel Decrypt(byte[] data);
    byte[] Encrypt(InvoiceModel invoice);

    ClientSideEncryptionOptions GetBlobEncryptionOptions();
}

internal class InvoiceCrypto : IInvoiceCrypto
{
    private readonly KeyClient _keyClient;
    private readonly string _keyName;
    private readonly Credentials _cred;

    public InvoiceCrypto(IConfiguration config, Credentials cred)
    {
        _cred = cred;
        _keyClient = new KeyClient(new(config["KeyLocation"]), cred.Get);
        _keyName = config["KeyName"];
    }

    public InvoiceModel Decrypt(byte[] data)
    {
        var cryptoClient = GetCryptoClient();
        var decryptResult = cryptoClient.Decrypt(EncryptionAlgorithm.RsaOaep, data);
        return System.Text.Json.JsonSerializer.Deserialize<InvoiceModel>(Encoding.UTF8.GetString(decryptResult.Plaintext));
    }

    public byte[] Encrypt(InvoiceModel invoice)
    {
        var cryptoClient = GetCryptoClient();
        var jsonData = System.Text.Json.JsonSerializer.Serialize(invoice);
        var decryptResult = cryptoClient.Encrypt(EncryptionAlgorithm.RsaOaep, Encoding.UTF8.GetBytes(jsonData));
        return decryptResult.Ciphertext;
    }

    private CryptographyClient GetCryptoClient()
    {
        var key = _keyClient.GetKey(_keyName);
        return new CryptographyClient(key.Value.Id, _cred.Get);
    }

    public ClientSideEncryptionOptions GetBlobEncryptionOptions()
    {
        return new ClientSideEncryptionOptions(ClientSideEncryptionVersion.V2_0)
        {
            KeyEncryptionKey = GetCryptoClient(),
            KeyResolver = new KeyResolver(_cred.Get),
            // String value that the client library will use when calling IKeyEncryptionKey.WrapKey()
            KeyWrapAlgorithm = "RSA-OAEP"
        };
    }
}

internal class Credentials
{
    private readonly Azure.Core.TokenCredential _credentials;
    public Credentials(IConfiguration config)
    {
        _credentials = new ClientSecretCredential(
            config["azTenant"],
            config["azClientId"],
            config["azSecret"]);
    }

    public Azure.Core.TokenCredential Get => _credentials;
}
