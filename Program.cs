using InvoiceToPdf.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(config => 
    {
    })
    .ConfigureServices(config =>
    {
        config.AddSingleton<IInvoiceStore, InvoiceStore>();
        config.AddSingleton<IBlobCrypto, BlobCrypto>();
        config.AddScoped<ICredentials, Credentials>();
    })
    .Build();

host.Run();
