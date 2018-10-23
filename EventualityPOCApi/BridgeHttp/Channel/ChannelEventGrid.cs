using EventualityPOCApi.Shared.Framework;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace EventualityPOCApi.Gateway.BridgeHttp.Channel
{
    public class ChannelEventGrid : IChannel
    {
        private readonly EventGridClient _eventGridClient;
        protected Subject<StatementWrapper> _subject;
        private readonly string _topicHostName = new Uri("https://personcontext-perception.westeurope-1.eventgrid.azure.net/api/events").Host;
        
            #region Constructor
        public ChannelEventGrid(EventGridClient eventGridClient)
        {
            _eventGridClient = eventGridClient;
            _subject = new Subject<StatementWrapper>();

            // TODO - push to subject when events come in from event grid
        }
        #endregion

        #region Public
        public IObservable<StatementWrapper> Observable()
        {
            return _subject;
        }

        public Task NextAsync(StatementWrapper statementWrapper)
        {
            var events = new List<EventGridEvent>
            {
                new EventGridEvent()
                {
                    Data = statementWrapper.Data,
                    DataVersion = statementWrapper.DataVersion,
                    EventTime = statementWrapper.EventTime,
                    EventType = statementWrapper.EventType,
                    Id = statementWrapper.Id,
                    Subject = statementWrapper.Subject,
                }
            };

            return _eventGridClient.PublishEventsAsync(_topicHostName, events);
        }

        public void RegisterHandler(Action<StatementWrapper> handler)
        {
            _subject.Subscribe(handler);
        }
        #endregion
    }

    public class DecisionChannelEventGrid : ChannelEventGrid, IDecisionChannel
    {
        #region Constructor
        public DecisionChannelEventGrid(EventGridClient eventGridClient) : base(eventGridClient) { }
        #endregion
    }

    public class PerceptionChannelEventGrid : ChannelEventGrid, IPerceptionChannel
    {
        #region Constructor
        public PerceptionChannelEventGrid(EventGridClient eventGridClient) : base(eventGridClient) { }
        #endregion
    }
}
