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
        private HttpResponseData postHandling(HttpRequestData req){
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            // response.WriteString("Welcome to Azure Functions haha!");

            MessResponse? data =  JsonSerializer.Deserialize<MessResponse>(req.Body); 
            response.WriteString("hi" + data.ToString());
            return response;
        }

        private HttpResponseData getHandling(HttpRequestData req){
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            Dictionary<String, String>? cred = JsonSerializer.Deserialize<Dictionary<String, String>>(File.ReadAllText("./media.json"));

            String? mode = req.Query["hub.mode"];
            String? token = req.Query["hub.verify_token"];
            String? challenge = req.Query["hub.challenge"];

            if (mode == "subscribe" && token == cred["verifyToken"]){
                response.WriteString(challenge);
                return response;
            }
            return req.CreateResponse(HttpStatusCode.Forbidden);
        }
        
        private readonly ILogger _logger;

        public DatakubeHttpTrigger(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<DatakubeHttpTrigger>();
        }

        [Function("DatakubeHttpTrigger")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            if (req.Method == "POST"){
                return this.postHandling(req);
            }
            if (req.Method == "GET"){
                return this.getHandling(req);
            }
            return req.CreateResponse(HttpStatusCode.NotFound);
        }
    }
}
