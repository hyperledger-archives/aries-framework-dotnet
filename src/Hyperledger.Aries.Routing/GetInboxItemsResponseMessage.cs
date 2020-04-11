using System;
using System.Collections.Generic;
using Hyperledger.Aries.Agents;

namespace Hyperledger.Aries.Routing
{
    public class GetInboxItemsResponseMessage : AgentMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetInboxItemsResponseMessage"/> class.
        /// </summary>
        public GetInboxItemsResponseMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = RoutingTypeNames.GetInboxItemsResponseMessage;
        }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public IEnumerable<InboxItemMessage> Items { get; set; }
    }
}