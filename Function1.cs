using InvoiceToPdf.Invoices;
using InvoiceToPdf.Storage;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using System.Net;
using System.Text.Json;

namespace InvoiceToPdf
{
    public class Function1
    {
        private readonly ILogger _logger;
        private readonly IInvoiceStore _invoiceStore;

        public Function1(ILoggerFactory loggerFactory, IInvoiceStore invoiceStore)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
            _invoiceStore = invoiceStore;
        }

        [Function("Function1")]
        public HttpResponseData Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "put", Route = "{invoiceNumber}")] HttpRequestData req,
            string invoiceNumber,
            ExecutionContext context)
        {
            var response = req.Method switch
            {
                "GET" => GetResponse(req, invoiceNumber),
                "PUT" => PutResponse(req, invoiceNumber),
                _ => ErrorResponse(req)
            };

            return response;
        }

        private HttpResponseData ErrorResponse(HttpRequestData req)
        {
            _logger.LogError("Error!");
            var response = HttpResponseData.CreateResponse(req);
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.Headers.Add("Content-Type", "text");
            return response;
        }

        private HttpResponseData PutResponse(HttpRequestData req, string invoiceNumber)
        {
            _logger.LogError($"Put Invoice {invoiceNumber}");
            using var streamReader = new StreamReader(req.Body);
            var requestBody = streamReader.ReadToEnd();
            var model = JsonSerializer.Deserialize<InvoiceModel>(requestBody);

            if(model is null)
            {
                return ErrorResponse(req);
            }

            _invoiceStore.SaveInvoice(model);
            return GenerateDocumentResponse(req, model);
        }

        private HttpResponseData GetResponse(HttpRequestData req, string invoiceNumber)
        {
            _logger.LogError($"Get Invoice {invoiceNumber}");
            var model = _invoiceStore.GetInvoice(invoiceNumber);
            //var model = new InvoiceModel
            //{
            //    InvoiceNumber = "Bentham",
            //    Items = new List<OrderItem>
            //    {
            //        new OrderItem
            //        {
            //            Name = "magic beans",
            //            Price = 10,
            //            VAT = 20,
            //            Quantity = 5,
            //        },
            //        new OrderItem
            //        {
            //            Name = "magic horse",
            //            Price = 468.99m,
            //            VAT = 20,
            //            Quantity = 1,
            //        },
            //        new OrderItem
            //        {
            //            Name = "so many treats",
            //            Price = 20,
            //            VAT = 20,
            //            Quantity = 1000,
            //        }
            //    },
            //    Comments = "I hunger. My tummy. It rumbles. I am left to eat toilet roll and carpet fluff."
            //};

            return GenerateDocumentResponse(req, model);
        }

        private HttpResponseData GenerateDocumentResponse(HttpRequestData req, InvoiceModel model)
        {
            _logger.LogError($"Generate PDF");
            var invoice = new InvoiceDocument(model);
            var data = invoice.GeneratePdf();

            var response = HttpResponseData.CreateResponse(req);
            response.StatusCode = HttpStatusCode.OK;
            response.Headers.Add("Content-Type", "application/pdf");
            response.Body = new MemoryStream(data);
            return response;
        }
    }
}
