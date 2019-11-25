using Hyperledger.Aries.Features.DidExchange;

namespace Hyperledger.Aries.Routing
{
    public class AgentPublicConfiguration
    {
        public string ServiceEndpoint { get; set; }

        public string RoutingKey { get; set; }

        public ConnectionInvitationMessage Invitation { get; set; }
    }
}