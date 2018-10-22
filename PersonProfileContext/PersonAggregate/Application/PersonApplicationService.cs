using EventualityPOC.PersonProfileContext.PersonAggregate.Domain;
using EventualityPOCApi.Shared.Xapi;

namespace EventualityPOC.PersonProfileContext.PersonAggregate.Application
{
    public class PersonApplicationService
    {
        public static StatementExtension PersonCreationRequested(StatementExtension perceptionStatement, IPersonRepository personRepository)
        {
            personRepository.SavePerceptionAsync(perceptionStatement);

            var person = perceptionStatement.targetData<Person>();
            person.PopulateId();

            var decisionStatement = Person.CreatePerson(perceptionStatement, person);
            personRepository.SaveDecisionAsync(decisionStatement);

            return decisionStatement;
        }

        public static StatementExtension PersonRequested(StatementExtension perceptionStatement, IPersonRepository personRepository)
        {
            var idOfPersonRequested = perceptionStatement.targetId();
            var person = personRepository.RetrievePerson(idOfPersonRequested);

            return Person.RetrievePerson(perceptionStatement, person);
        }

        public static StatementExtension PersonUpdateRequested(StatementExtension perceptionStatement, IPersonRepository personRepository)
        {
            personRepository.SavePerceptionAsync(perceptionStatement);

            var idOfPerson = perceptionStatement.targetId();
            var person = personRepository.RetrievePerson(idOfPerson);

            var decisionStatement = Person.UpdatePerson(perceptionStatement, person);
            personRepository.SaveDecisionAsync(decisionStatement);

            return decisionStatement;
        }
    }
}
