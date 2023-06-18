using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Files.DataLake;
using Azure.Storage;
using System.IO;
using System.Text.Json;

namespace DataKube.Function
{
    public class DatakubeHttpTrigger
    {
        private readonly ILogger _logger;

        public DatakubeHttpTrigger(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<DatakubeHttpTrigger>();
        }

        [Function("DatakubeHttpTrigger")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            // response.WriteString("Welcome to Azure Functions haha!");

            Message? data =  JsonSerializer.Deserialize<Message>(req.Body);
            
            response.WriteString("hi" + data.a);

            return response;
        }
    }
}
