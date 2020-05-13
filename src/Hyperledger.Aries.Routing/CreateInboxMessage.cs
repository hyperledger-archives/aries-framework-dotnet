using Hyperledger.Aries.Agents;
using System;
using System.Collections.Generic;

namespace Hyperledger.Aries.Routing
{
    /// <summary>
    /// Create Inbox Message
    /// </summary>
    /// <seealso cref="Hyperledger.Aries.Agents.AgentMessage" />
    public class CreateInboxMessage : AgentMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateInboxMessage"/> class.
        /// </summary>
        public CreateInboxMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = RoutingTypeNames.CreateInboxMessage;
        }

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        public Dictionary<string, string> Metadata { get; set; }
    }
}