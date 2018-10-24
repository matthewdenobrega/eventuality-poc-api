using EventualityPOCApi.Gateway.Configuration;
using EventualityPOCApi.Shared.Framework;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventualityPOCApi.Gateway.Channel
{
    public class ChannelEventGrid : IChannel
    {
        private readonly EventGridClient _eventGridClient;
        private Action<StatementWrapper> _handler;
        protected string _topicHostName;
        
        #region Constructor
        public ChannelEventGrid(EventGridClient eventGridClient, EventGridConfiguration eventGridConfiguration)
        {
            _eventGridClient = eventGridClient;
        }
        #endregion

        #region Public
        public IObservable<StatementWrapper> Observable()
        {
            throw new NotImplementedException(); // Events come in through the EventGridEventHandlerController
        }

        public Task NextAsync(StatementWrapper statementWrapper)
        {
            TriggerHandler(statementWrapper);

            var events = CreateEventGridEventList(statementWrapper);

            return _eventGridClient.PublishEventsAsync(_topicHostName, events);
        }

        public void RegisterHandler(Action<StatementWrapper> handler)
        {
            _handler = handler;
        }

        public void TriggerHandler(StatementWrapper statementWrapper)
        {
            if (_handler != null) _handler.Invoke(statementWrapper);
        }
        #endregion

        #region private
        private List<EventGridEvent> CreateEventGridEventList(StatementWrapper statementWrapper)
        {
            if (statementWrapper == null) return new List<EventGridEvent>();

            return new List<EventGridEvent>
                {
                    new EventGridEvent()
                    {
                        Data = statementWrapper.Data.ToJObject(),
                        DataVersion = statementWrapper.DataVersion,
                        EventTime = statementWrapper.EventTime,
                        EventType = statementWrapper.EventType,
                        Id = statementWrapper.Id,
                        Subject = statementWrapper.Subject,
                    }
                };
        }
        #endregion
    }

    public class DecisionChannelEventGrid : ChannelEventGrid, IDecisionChannel
    {
        #region Constructor
        public DecisionChannelEventGrid(EventGridClient eventGridClient, EventGridConfiguration eventGridConfiguration) : base(eventGridClient, eventGridConfiguration)
        {
            // The gateway does not push to the decision channel so the topic host name is not needed here
        }
        #endregion
    }

    public class PerceptionChannelEventGrid : ChannelEventGrid, IPerceptionChannel
    {
        #region Constructor
        public PerceptionChannelEventGrid(EventGridClient eventGridClient, EventGridConfiguration eventGridConfiguration) : base(eventGridClient, eventGridConfiguration)
        {
            _topicHostName = new Uri(eventGridConfiguration.PersonProfileContextPerceptionTopicUrl).Host;
        }
        #endregion
    }
}
