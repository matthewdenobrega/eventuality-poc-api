using EventualityPOC.PersonProfileContext.PersonAggregate.Domain;
using EventualityPOCApi.Shared.Xapi;
using Newtonsoft.Json.Linq;
using System;

namespace EventualityPOC.PersonProfileContext.PersonAggregate.Application
{
    public class PersonApplicationService
    {
        public static JObject PersonCreationRequested(JObject incomingJObject, IPersonRepository personRepository)
        {
            var statementPersonCreationRequested = new StatementExtension(incomingJObject);
            personRepository.SavePerception(statementPersonCreationRequested);

            var person = statementPersonCreationRequested.targetData<Person>();
            person.PopulateId();

            var statementPersonCreated = statementPersonCreationRequested.createSuccessor(
                new Uri(Verb.PersonCreated), person, person.Id);
            personRepository.SaveDecision(statementPersonCreated);

            return statementPersonCreated.ToJObject();
        }

        public static JObject PersonRequested(JObject incomingJObject, IPersonRepository personRepository)
        {
            var statementPersonRequested = new StatementExtension(incomingJObject);

            var idOfPersonRequested = statementPersonRequested.targetId();
            var person = personRepository.RetrievePerson(idOfPersonRequested);

            var statementPersonRetrieved = statementPersonRequested.createSuccessor(
                new Uri(Verb.PersonRetrieved), person);

            return statementPersonRetrieved.ToJObject();
        }

        public static JObject PersonUpdateRequested(JObject incomingJObject, IPersonRepository personRepository)
        {
            var statementPersonUpdateRequested = new StatementExtension(incomingJObject);
            personRepository.SavePerception(statementPersonUpdateRequested);

            var statementPersonUpdated = statementPersonUpdateRequested.createSuccessor(new Uri(Verb.PersonUpdated),
                statementPersonUpdateRequested.targetData<Person>());
            personRepository.SaveDecision(statementPersonUpdated);

            return statementPersonUpdated.ToJObject();
        }
    }
}
