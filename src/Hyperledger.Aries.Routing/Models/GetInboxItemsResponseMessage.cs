using System;
using System.Collections.Generic;
using Hyperledger.Aries.Agents;

namespace Hyperledger.Aries.Routing
{
    public class GetInboxItemsResponseMessage : AgentMessage
    {
        public GetInboxItemsResponseMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = RoutingTypeNames.GetInboxItemsResponseMessage;
        }

        public IEnumerable<InboxItemMessage> Items { get; set; }
    }
}