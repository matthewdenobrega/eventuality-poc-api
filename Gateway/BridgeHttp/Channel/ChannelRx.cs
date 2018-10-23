using EventualityPOCApi.Shared.Framework;
using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace EventualityPOCApi.Gateway.BridgeHttp.Channel
{
    public class ChannelRx
    {
        protected Subject<StatementWrapper> _subject;

        #region Constructor
        public ChannelRx()
        {
            _subject = new Subject<StatementWrapper>();
        }
        #endregion

        #region Public
        public IObservable<StatementWrapper> Observable()
        {
            return _subject;
        }

        public Task NextAsync(StatementWrapper statementWrapper)
        {
            _subject.OnNext(statementWrapper);

            return Task.FromResult<object>(null);
        }

        public void RegisterHandler(Action<StatementWrapper> handler)
        {
            _subject.Subscribe(handler);
        }
        #endregion
    }

    public class DecisionChannelRx : ChannelRx, IDecisionChannel
    {
        #region Constructor
        public DecisionChannelRx() : base() { }
        #endregion
    }

    public class PerceptionChannelRx : ChannelRx, IPerceptionChannel
    {
        #region Constructor
        public PerceptionChannelRx() : base() { }
        #endregion
    }
}
