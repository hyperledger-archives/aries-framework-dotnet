using Hyperledger.Aries.Agents;
using System;
using System.Collections.Generic;

namespace Hyperledger.Aries.Routing
{
    public class DeleteInboxItemsMessage : AgentMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteInboxItemsMessage"/> class.
        /// </summary>
        public DeleteInboxItemsMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = RoutingTypeNames.DeleteInboxItemsMessage;
        }

        /// <summary>
        /// Gets or sets the inbox item ids.
        /// </summary>
        /// <value>
        /// The inbox item ids.
        /// </value>
        public IEnumerable<string> InboxItemIds { get; set; }
    }
}