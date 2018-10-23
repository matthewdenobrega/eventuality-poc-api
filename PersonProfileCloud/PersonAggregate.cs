using EventualityPOCApi.Context.PersonProfileContext.PersonAggregate.Application;
using EventualityPOCApi.Context.PersonProfileContext.PersonAggregate.Framework;
using EventualityPOCApi.Shared.Framework;
using EventualityPOCApi.Shared.Xapi;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;

namespace EventualityPOCApi.Cloud.PersonProfileCloud
{
    public static class PersonAggregate
    {
        private static readonly string _cosmosDBAccountEndpoint = Environment.GetEnvironmentVariable("CosmosDBAccountEndpoint");
        private static readonly string _cosmosDBAccountKey = Environment.GetEnvironmentVariable("CosmosDBAccountKey");
        private static readonly DocumentClient _documentClient = new DocumentClient(new Uri(_cosmosDBAccountEndpoint), _cosmosDBAccountKey);
        private static readonly IPersonRepository _personRepository = new PersonRepositoryCosmosDb(_documentClient);

        [FunctionName("PersonAggregate")]
        public static void Run([EventGridTrigger]JObject eventGridEventJObject, ILogger logger)
        {
            logger.LogInformation("PersonAggregate Event grid trigger function processed a request.");

            var perceptionStatement = new StatementExtension((JObject)eventGridEventJObject.GetValue("Data"));
            var subject = eventGridEventJObject.GetValue("Subject").ToString();
            var verb = eventGridEventJObject.GetValue("EventType").ToString();

            try
            {
                StatementExtension decisionStatement = null;

                switch (verb)
                {
                    case Verb.PersonCreationRequested:
                        decisionStatement = PersonApplicationService.PersonCreationRequested(perceptionStatement, _personRepository);
                        break;
                    case Verb.PersonRequested:
                        decisionStatement = PersonApplicationService.PersonRequested(perceptionStatement, _personRepository);
                        break;
                    case Verb.PersonUpdateRequested:
                        decisionStatement = PersonApplicationService.PersonUpdateRequested(perceptionStatement, _personRepository);
                        break;
                }

                if (decisionStatement == null) return;

                var decisionStatementWrapper = new StatementWrapper(subject, decisionStatement);
                // _decisionChannel.NextAsync(decisionStatementWrapper);
            }
            catch (Exception exception)
            {
                logger.LogError("Exception handling statement for person aggregate: " + exception.Message);
                logger.LogError(exception.StackTrace);
            }
        }
    }
}
