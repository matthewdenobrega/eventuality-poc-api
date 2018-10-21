using EventualityPOCApi.Gateway.BridgeHttp.Channel;
using EventualityPOCApi.Shared.Framework;
using Microsoft.AspNetCore.SignalR;

namespace EventualityPOCApi.Gateway.BridgeHttp.TransportAdapter
{
    public class HubPublisherWebsocket
    {
        private IDecisionChannel _decisionChannel;
        private IHubContext<HubSubscriberWebsocket> _hubSubContext;

        #region Constructor
        public HubPublisherWebsocket(IHubContext<HubSubscriberWebsocket> hubContext, IDecisionChannel decisionChannel)
        {
            _hubSubContext = hubContext;
            _decisionChannel = decisionChannel;
        }
        #endregion

        #region Public
        public void RegisterOutgoingHandler()
        {
            _decisionChannel.RegisterHandler(PushEventToHub);
        }
        #endregion

        #region private
        private void PushEventToHub(StatementWrapper statementWrapper)
        {
            _hubSubContext.Clients.Client(statementWrapper.Subject).SendAsync("Decision", statementWrapper.Data);
        }
        #endregion
    }
}
