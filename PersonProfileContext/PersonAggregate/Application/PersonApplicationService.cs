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
            var personCreationRequestedStatement = new StatementExtension(incomingJObject);
            personRepository.SavePerception(personCreationRequestedStatement);

            var person = personCreationRequestedStatement.targetData<Person>();
            person.PopulateId();

            var responseStatement = person.CreatePerson(personCreationRequestedStatement);
            personRepository.SaveDecision(responseStatement);

            return responseStatement.ToJObject();
        }

        public static JObject PersonRequested(JObject incomingJObject, IPersonRepository personRepository)
        {
            var personRequestedStatement = new StatementExtension(incomingJObject);

            var idOfPersonRequested = personRequestedStatement.targetId();
            var person = personRepository.RetrievePerson(idOfPersonRequested);

            var responseStatement = Person.RetrievePerson(personRequestedStatement, person);

            return responseStatement.ToJObject();
        }

        public static JObject PersonUpdateRequested(JObject incomingJObject, IPersonRepository personRepository)
        {
            var personUpdateRequestedStatement = new StatementExtension(incomingJObject);
            personRepository.SavePerception(personUpdateRequestedStatement);

            var idOfPerson = personUpdateRequestedStatement.targetId();
            var person = personRepository.RetrievePerson(idOfPerson);

            var responseStatement = Person.UpdatePerson(personUpdateRequestedStatement, person);
            personRepository.SaveDecision(responseStatement);

            return responseStatement.ToJObject();
        }
    }
}
