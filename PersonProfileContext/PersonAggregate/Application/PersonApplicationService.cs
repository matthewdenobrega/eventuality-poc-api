using EventualityPOCApi.Context.PersonProfileContext.PersonAggregate.Domain;
using EventualityPOCApi.Shared.Xapi;
using System.Threading.Tasks;

namespace EventualityPOCApi.Context.PersonProfileContext.PersonAggregate.Application
{
    public class PersonApplicationService
    {
        #region Public
        public static async Task<StatementExtension> MakeDecisionAsync(StatementExtension perceptionStatement, IPersonRepository personRepository)
        {
            await personRepository.SavePerceptionAsync(perceptionStatement);

            StatementExtension decisionStatement = null;

            switch (perceptionStatement?.verb?.id?.ToString())
            {
                case Verb.PersonCreationRequested:
                    decisionStatement = PersonCreationRequested(perceptionStatement, personRepository);
                    break;
                case Verb.PersonRequested:
                    decisionStatement = PersonRequested(perceptionStatement, personRepository);
                    break;
                case Verb.PersonUpdateRequested:
                    decisionStatement = PersonUpdateRequested(perceptionStatement, personRepository);
                    break;
            }

            await personRepository.SaveDecisionAsync(decisionStatement);

            return decisionStatement;
        }
        #endregion

        #region Private
        private static StatementExtension PersonCreationRequested(StatementExtension perceptionStatement, IPersonRepository personRepository)
        {
            var person = perceptionStatement.targetData<Person>();

            return Person.CreatePerson(perceptionStatement, person);
        }

        private static StatementExtension PersonRequested(StatementExtension perceptionStatement, IPersonRepository personRepository)
        {
            var idOfPersonRequested = perceptionStatement.targetId();
            var person = personRepository.RetrievePerson(idOfPersonRequested);

            return Person.RetrievePerson(perceptionStatement, person);
        }

        private static StatementExtension PersonUpdateRequested(StatementExtension perceptionStatement, IPersonRepository personRepository)
        {
            var idOfPerson = perceptionStatement.targetId();
            var person = personRepository.RetrievePerson(idOfPerson);

            return Person.UpdatePerson(perceptionStatement, person);
        }
        #endregion
    }
}