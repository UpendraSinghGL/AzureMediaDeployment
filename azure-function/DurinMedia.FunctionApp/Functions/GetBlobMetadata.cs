using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sample.MediaInfo.Core;

namespace Sample.Mediainfo.FxnApp.Functions
{
    /// <summary>
    /// Azure Function to get a MediaInfo report from a BlobUri.
    /// </summary>
    public class GetBlobMetadata
    {
        [FunctionName("GetBlobMetadata")]
        public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
    ILogger log)
        {
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING_VENDOR");
            string BlobPath = req.Query["BlobPath"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            BlobPath = BlobPath ?? data?.BlobPath;
            int exists = 0;

            if (!string.IsNullOrEmpty(connectionString) && !string.IsNullOrEmpty(BlobPath))
            {
                string[] fpArray = BlobPath.Split("/");
                string container = fpArray[0];
                string blobName = BlobPath.Substring(BlobPath.IndexOf('/') + 1);

                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
                BlobContainerClient blobCont = blobServiceClient.GetBlobContainerClient(container);

                BlobClient blobClient = blobCont.GetBlobClient(blobName);
                BlobProperties properties = await blobClient.GetPropertiesAsync();

                if(properties.Metadata.ContainsKey("Filetype") && properties.Metadata.ContainsKey("Source"))
                {
                    exists = 1;
                }

                return new OkObjectResult(exists);
            }
            else
            {
                return new NotFoundObjectResult("connection string or BlobPath is missing");
            }

        }

        /*
        To test this function:
        curl -X POST \
            'http://localhost:7071/api/GetBlobMetadata' \
            -H 'Content-Type: application/json' \
            -H 'cache-control: no-cache' \
            -d '
            {
                "blobUri": "https://youraccount.blob.core.windows.net/test/bbb.mp4"
            }
            ' -v
        Or:
        curl -X POST \
            'http://localhost:7071/api/MediaInfo?blobUri=https://youraccount.blob.core.windows.net/test/bbb.mp4' \
            -H 'Content-Type: application/json' -H 'cache-control: no-cache' -d '' -v
        */
    }
}
