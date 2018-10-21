using EventualityPOCApi.Gateway.BridgeHttp.Channel;
using EventualityPOCApi.Shared.Framework;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace EventualityPOCApi.Gateway.BridgeHttp.TransportAdapter
{
    public class HubSubscriberWebsocket : Hub
    {
        private readonly IPerceptionChannel _perceptionChannel;

        #region Constructor
        public HubSubscriberWebsocket(IPerceptionChannel perceptionChannel)
        {
            _perceptionChannel = perceptionChannel;
        }
        #endregion

        #region Public
        public async Task Perception(JObject payload)
        {
            var statementWrapper = new StatementWrapper(this.Context.ConnectionId, payload);
            await _perceptionChannel.NextAsync(statementWrapper);
        }
        #endregion
    }
}
