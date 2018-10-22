using EventualityPOCApi.Shared.Xapi;
using System;

namespace EventualityPOC.PersonProfileContext.PersonAggregate.Domain
{
    public class Person
    {
        public string Id { get; set; }
        public string Name { get; set; }

        #region Static
        public static StatementExtension RetrievePerson(StatementExtension personRequestedStatement, Person person)
        {
            return person != null ?
                personRequestedStatement.createSuccessor(new Uri(Verb.PersonRetrieved), person) :
                personRequestedStatement.createSuccessor(new Uri(Verb.PersonRetrievalFailed), null);
        }

        public static StatementExtension UpdatePerson(StatementExtension personRequestedStatement, Person person)
        {
            return person?.Name != null ?
                personRequestedStatement.createSuccessor(new Uri(Verb.PersonUpdated), person) :
                personRequestedStatement.createSuccessor(new Uri(Verb.PersonUpdateFailed), null);
        }
        #endregion

        #region Public
        public StatementExtension CreatePerson(StatementExtension personCreationRequestedStatement)
        {
            return Name != null ?
                personCreationRequestedStatement.createSuccessor(new Uri(Verb.PersonCreated), this, Id) :
                personCreationRequestedStatement.createSuccessor(new Uri(Verb.PersonCreationFailed), this);
        }

        public void PopulateId()
        {
            Id = Id ?? "http://eventuality.poc/person/" + Guid.NewGuid().ToString();
        }
        #endregion
    }
}
