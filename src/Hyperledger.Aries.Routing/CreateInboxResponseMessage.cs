using Hyperledger.Aries.Agents;
using System;

namespace Hyperledger.Aries.Routing
{
    public class CreateInboxResponseMessage : AgentMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateInboxResponseMessage"/> class.
        /// </summary>
        public CreateInboxResponseMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = RoutingTypeNames.CreateInboxResponseMessage;
        }

        /// <summary>
        /// Gets or sets the inbox identifier.
        /// </summary>
        /// <value>
        /// The inbox identifier.
        /// </value>
        public string InboxId { get; set; }

        /// <summary>
        /// Gets or sets the inbox key.
        /// </summary>
        /// <value>
        /// The inbox key.
        /// </value>
        public string InboxKey { get; set; }
    }
}