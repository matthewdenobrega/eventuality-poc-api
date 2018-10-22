using EventualityPOC.PersonProfileContext.PersonAggregate.Domain;
using EventualityPOCApi.Shared.Xapi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EventualityPOCApi.PersonProfileContextTest.PersonAggregate.Domain.PersonTest
{
    [TestClass]
    public class RetrievePersonTest
    {
        [TestMethod]
        public void Retrieve_person_should_fail_if_the_person_is_not_found()
        {
            var personRequestedStatement = new StatementExtension();

            var responseStatement = Person.RetrievePerson(personRequestedStatement, null);

            Assert.AreEqual(responseStatement.verb.id, Verb.PersonRetrievalFailed);
        }

        [TestMethod]
        public void Retrieve_person_should_return_the_person_if_the_person_is_found()
        {
            var personRequestedStatement = new StatementExtension();
            var person = new Person()
            {
                Name = "Test"
            };

            var responseStatement = Person.RetrievePerson(personRequestedStatement, person);

            Assert.AreEqual(responseStatement.verb.id, Verb.PersonRetrieved);
            Assert.AreEqual(responseStatement.targetData<Person>().Name, person.Name);
        }
    }
}
