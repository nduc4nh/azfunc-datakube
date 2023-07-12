using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Files.DataLake;
using Azure.Storage;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace DataKube.Function
{
    public class DatakubeHttpTrigger
    {
        private HttpResponseData postHandling(HttpRequestData req){
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            // response.WriteString("Welcome to Azure Functions haha!");

            MessResponse? data =  JsonSerializer.Deserialize<MessResponse>(req.Body); 
             string responseJson = JsonSerializer.Serialize(data);
            string user = data.getReceiverId();
            string time = DateTime.Now.ToString("MM_dd_yyyy");

            response.WriteString("hi" + data.ToString());

            StorageSharedKeyCredential cred = new StorageSharedKeyCredential("datakube", _configuration["accessToken"]);
            String dfsUri = _configuration["datalakeUrl"];
            
            //Create File System client using DataLakeServiceClient
            DataLakeServiceClient serviceClient = new DataLakeServiceClient(new Uri(dfsUri), cred);
            DataLakeFileSystemClient fsClient = serviceClient.GetFileSystemClient("landing");

            DataLakeDirectoryClient dirClient = fsClient.GetDirectoryClient("landing");
            if (!dirClient.Exists()) dirClient.Create();

            DataLakeDirectoryClient conversationDirClient  = dirClient.GetSubDirectoryClient("conversation");
            if (!conversationDirClient.Exists()) conversationDirClient.Create();

            DataLakeDirectoryClient userDirClient = conversationDirClient.GetSubDirectoryClient(user); 
            if (!userDirClient.Exists()) userDirClient.Create();

            DataLakeDirectoryClient userDateDirClient = userDirClient.GetSubDirectoryClient(time); 
            if (!userDateDirClient.Exists()) userDateDirClient.Create();

            DataLakeFileClient fileClient = userDateDirClient.GetFileClient("user_conversation.json");
            // fileClient.Create();

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(responseJson));
            fileClient.Upload(stream, true);

            return response;
        }

        private HttpResponseData getHandling(HttpRequestData req){
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            // Dictionary<String, String>? cred = JsonSerializer.Deserialize<Dictionary<String, String>>(File.ReadAllText("./media.json"));

            String? mode = req.Query["hub.mode"];
            String? token = req.Query["hub.verify_token"];
            String? challenge = req.Query["hub.challenge"];
            String? realToken = _configuration["verifyKey"];
            if (mode == "subscribe" && token == realToken){
                response.WriteString(challenge);
                return response;
            }
            
            Console.WriteLine($"{realToken}");

            return req.CreateResponse(HttpStatusCode.Forbidden);
        }
        
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        public DatakubeHttpTrigger(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<DatakubeHttpTrigger>();
            _configuration = configuration;
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
