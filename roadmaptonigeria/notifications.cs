using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Specialized;
using System.Linq;
using System.IO;
using Xenhey.BPM.Core.Net8.Implementation;
using Xenhey.BPM.Core.Net8;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace roadmaptonigeria
{
    public class notifications
    {
        private readonly ILogger _logger;

        public notifications(ILogger<document> logger)
        {
            _logger = logger;
        }

        private HttpRequest _req;
        private NameValueCollection nvc = new NameValueCollection();
        [Function("notifications")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous,"get", Route = "notifications/{tokenid}")]
            HttpRequest req, string tokenid)
        {
            var input = JsonConvert.SerializeObject(new { tokenid });
            _req = req;
            string ApiKeyName = "x-api-key";
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            string requestBody = input;
            _req.Headers.ToList().ForEach(item => { nvc.Add(item.Key, item.Value.FirstOrDefault()); });
            nvc.Add(ApiKeyName, "D40DEA1D8C9042BE83AB711AD8F0FA6E");
            var results = orchrestatorService.ReturnFile(requestBody);
            return resultSet(results);

        }

        private ActionResult resultSet(byte[] reponsePayload)
        {
            var mediaSelectedtype = nvc.Get("Content-Type");
            var returnContent = new FileContentResult(reponsePayload, mediaSelectedtype);
            return returnContent;
        }
        private IOrchestrationService orchrestatorService
        {
            get
            {
                return new RemoteOrchrestratorService(nvc);
            }
        }

    }
}
