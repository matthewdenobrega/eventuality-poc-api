using Newtonsoft.Json.Linq;
using System;
using TinCan;

namespace EventualityPOCApi.Shared.Framework
{
    public class StatementWrapper
    {
        public JObject Data { get; }
        public string DataVersion { get; }
        public DateTime EventTime { get; }
        public string EventType { get; }
        public string Id { get; }
        public string Subject { get; }

        #region Constructor
        public StatementWrapper(string connectionContextId, JObject jObject)
        {
            var statement = jObject != null ? new Statement(jObject) : throw new ArgumentException("Tried to create an event wrapper without a statement");

            Data = jObject;
            DataVersion = "1.0.0";
            EventTime = DateTime.Now;
            EventType = statement.verb.id.ToString();
            Id = Guid.NewGuid().ToString();
            Subject = connectionContextId;
        }
        #endregion
    }
}
