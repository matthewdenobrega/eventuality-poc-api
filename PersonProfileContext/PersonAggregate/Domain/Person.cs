using System;

namespace EventualityPOC.PersonProfileContext.PersonAggregate.Domain
{
    public class Person
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public void PopulateId()
        {
            Id = Id ?? "http://eventuality.poc/person/" + Guid.NewGuid().ToString();
        }
    }
}
