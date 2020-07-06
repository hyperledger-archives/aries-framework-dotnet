using Hyperledger.Aries.Agents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Hyperledger.Aries.Features.Discovery
{
    /// <summary>
    /// Represents a disclose message in the discovery protocol.
    /// </summary>
    public class DiscoveryDiscloseMessage : AgentMessage
    {
        /// <inheritdoc />
        public DiscoveryDiscloseMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypes.DiscoveryDiscloseMessageType;
            Protocols = new List<DisclosedMessageProtocol>();
        }

        /// <summary>
        /// A list of supported protocols.
        /// </summary>
        [JsonProperty("protocols")]
        public IList<DisclosedMessageProtocol> Protocols { get; set; }
    }

    /// <summary>
    /// Represents a disclosed message protocol object.
    /// </summary>
    public class DisclosedMessageProtocol
    {
        /// <summary>
        /// Protocol Identifier.
        /// </summary>
        [JsonProperty("pid")]
        [JsonPropertyName("pid")]
        public string ProtocolId { get; set; }

        /// <summary>
        /// Roles for the subject protocol.
        /// </summary>
        [JsonProperty("roles")]
        public IList<string> Roles { get; set; }
    }
}
