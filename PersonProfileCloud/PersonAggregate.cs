using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace EventualityPOCApi.Cloud.PersonProfileCloud
{
    public static class PersonAggregate
    {
        [FunctionName("PersonAggregate")]
        public static void Run([EventGridTrigger]JObject eventGridEventJObject, ILogger log)
        {
            log.LogInformation("PersonAggregate Event grid trigger function processed a request.");

            // var perceptionStatement = new StatementExtension(eventGridEventJObject.GetValue("data"));
        }
    }
}
