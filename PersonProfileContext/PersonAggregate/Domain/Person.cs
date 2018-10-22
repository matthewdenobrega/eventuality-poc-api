using EventualityPOCApi.Shared.Xapi;
using System;

namespace EventualityPOC.PersonProfileContext.PersonAggregate.Domain
{
    public class Person
    {
        public string Id { get; set; }
        public string Name { get; set; }

        #region Static
        public static StatementExtension CreatePerson(StatementExtension perceptionStatement, Person person)
        {
            if (perceptionStatement?.verb?.id?.ToString() != Verb.PersonCreationRequested)
            {
                throw new ArgumentException("Verb mismatch for creating person");
            }

            return person?.Name != null ?
                perceptionStatement.createSuccessor(new Uri(Verb.PersonCreated), person, person.Id) :
                perceptionStatement.createSuccessor(new Uri(Verb.PersonCreationFailed), person);
        }

        public static StatementExtension RetrievePerson(StatementExtension perceptionStatement, Person person)
        {
            if (perceptionStatement?.verb?.id?.ToString() != Verb.PersonRequested)
            {
                throw new ArgumentException("Verb mismatch for requesting person");
            }

            return person != null ?
                perceptionStatement.createSuccessor(new Uri(Verb.PersonRetrieved), person) :
                perceptionStatement.createSuccessor(new Uri(Verb.PersonRetrievalFailed), null);
        }

        public static StatementExtension UpdatePerson(StatementExtension perceptionStatement, Person person)
        {
            if (perceptionStatement?.verb?.id?.ToString() != Verb.PersonUpdateRequested)
            {
                throw new ArgumentException("Verb mismatch for updating person");
            }

            return person?.Name != null ?
                perceptionStatement.createSuccessor(new Uri(Verb.PersonUpdated), person) :
                perceptionStatement.createSuccessor(new Uri(Verb.PersonUpdateFailed), null);
        }
        #endregion

        #region Public
        public void PopulateId()
        {
            Id = Id ?? "http://eventuality.poc/person/" + Guid.NewGuid().ToString();
        }
        #endregion
    }
}
