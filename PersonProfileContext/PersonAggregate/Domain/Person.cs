using EventualityPOCApi.Shared.Xapi;
using System;

namespace EventualityPOCApi.Context.PersonProfileContext.PersonAggregate.Domain
{
    public class Person
    {
        public string Id { get; set; }
        public string Name { get; set; }
        private const string PersonRootUri = "http://eventuality.poc/person/";

        #region Static
        public static StatementExtension CreatePerson(StatementExtension perceptionStatement, Person person)
        {
            if (perceptionStatement?.verbString() != Verb.PersonCreationRequested) throw new ArgumentException("Incorrect verb to create person");

            person.PopulateId();

            return person?.Name != null ?
                perceptionStatement.createSuccessor(new Uri(Verb.PersonCreated), person, person.Id) :
                perceptionStatement.createSuccessor(new Uri(Verb.PersonCreationFailed), person);
        }

        public static StatementExtension RetrievePerson(StatementExtension perceptionStatement, Person person)
        {
            if (perceptionStatement?.verbString() != Verb.PersonRequested) throw new ArgumentException("Incorrect verb to retrieve person");

            return person != null ?
                perceptionStatement.createSuccessor(new Uri(Verb.PersonRetrieved), person) :
                perceptionStatement.createSuccessor(new Uri(Verb.PersonRetrievalFailed), null);
        }

        public static StatementExtension UpdatePerson(StatementExtension perceptionStatement, Person person)
        {
            if (perceptionStatement?.verbString() != Verb.PersonUpdateRequested) throw new ArgumentException("Incorrect verb to update person");

            return person?.Name != null ?
                perceptionStatement.createSuccessor(new Uri(Verb.PersonUpdated), person) :
                perceptionStatement.createSuccessor(new Uri(Verb.PersonUpdateFailed), null);
        }
        #endregion

        #region Private
        private void PopulateId()
        {
            Id = Id ?? PersonRootUri + Guid.NewGuid().ToString();
        }
        #endregion
    }
}