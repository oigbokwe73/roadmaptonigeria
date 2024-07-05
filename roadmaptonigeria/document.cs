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

namespace roadmaptonigeria
{
    public class document
    {
        private readonly ILogger _logger;

        public document(ILogger<document> logger)
        {
            _logger = logger;
        }

        private HttpRequest _req;
        private NameValueCollection nvc = new NameValueCollection();
        [Function("document")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post","get", Route = null)]
            HttpRequest req)
        {
            _req = req;

            _logger.LogInformation("C# HTTP trigger function processed a request.");
            string requestBody = await new StreamReader(_req.Body).ReadToEndAsync();
            _req.Headers.ToList().ForEach(item => { nvc.Add(item.Key, item.Value.FirstOrDefault()); });
            var results = orchrestatorService.ReturnFileAsStream(requestBody);
            return resultSet(results);

        }

        private ActionResult resultSet(Stream reponsePayload)
        {
            var mediaSelectedtype = nvc.Get("Content-Type");
            var returnContent = new FileStreamResult(reponsePayload, mediaSelectedtype);
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
