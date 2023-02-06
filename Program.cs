using InvoiceToPdf.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using static QuestPDF.Helpers.Colors;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(config => 
    {
    })
    .ConfigureServices(config =>
    {
        config.AddSingleton<IInvoiceStore, InvoiceStore>();
        config.AddSingleton<IInvoiceCrypto, InvoiceCrypto>();
        config.AddScoped<Credentials>();
    })
    .Build();

host.Run();
