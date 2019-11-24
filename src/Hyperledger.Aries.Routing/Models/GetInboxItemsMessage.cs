using System;
using Hyperledger.Aries.Agents;

namespace Hyperledger.Aries.Routing
{
    public class GetInboxItemsMessage : AgentMessage
    {
        public GetInboxItemsMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = RoutingTypeNames.GetInboxItemsMessage;
        }
    }
}