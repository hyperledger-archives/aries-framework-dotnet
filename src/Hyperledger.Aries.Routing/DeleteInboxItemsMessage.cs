using Hyperledger.Aries.Agents;
using System;
using System.Collections.Generic;

namespace Hyperledger.Aries.Routing
{
    public class DeleteInboxItemsMessage : AgentMessage
    {
        public DeleteInboxItemsMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = RoutingTypeNames.DeleteInboxItemsMessage;
        }

        public IEnumerable<string> InboxItemIds { get; set; }
    }
}