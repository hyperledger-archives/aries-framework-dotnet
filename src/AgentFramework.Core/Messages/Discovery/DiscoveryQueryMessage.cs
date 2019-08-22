using System;

namespace AgentFramework.Core.Messages.Discovery
{
    /// <summary>
    /// Represents a query message in the discovery protocol.
    /// </summary>
    public class DiscoveryQueryMessage : AgentMessage
    {
        /// <inheritdoc />
        public DiscoveryQueryMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypes.DiscoveryQueryMessageType;
        }

        /// <summary>
        /// Query for the discovery message.
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Comment for the discovery message.
        /// </summary>
        public string Comment { get; set; }
    }
}
