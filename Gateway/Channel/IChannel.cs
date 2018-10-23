using EventualityPOCApi.Shared.Framework;
using System;
using System.Threading.Tasks;

namespace EventualityPOCApi.Gateway.Channel
{
    public interface IChannel
    {
        IObservable<StatementWrapper> Observable();
        Task NextAsync(StatementWrapper statementWrapper);
        void RegisterHandler(Action<StatementWrapper> handler);
        void TriggerHandler(StatementWrapper statementWrapper);
    }

    public interface IDecisionChannel : IChannel { }
    public interface IPerceptionChannel : IChannel { }
}
