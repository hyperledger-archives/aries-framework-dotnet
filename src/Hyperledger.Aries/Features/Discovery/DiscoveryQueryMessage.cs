using System;
using Hyperledger.Aries.Agents;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.Discovery
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
        [JsonProperty("query")]
        public string Query { get; set; }

        /// <summary>
        /// Comment for the discovery message.
        /// </summary>
        [JsonProperty("comment")]
        public string Comment { get; set; }
    }
}
