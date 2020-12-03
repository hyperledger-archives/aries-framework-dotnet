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
        public DiscoveryDiscloseMessage() : base ()
        {
            Id = Guid.NewGuid().ToString();
            Type = UseMessageTypesHttps ? MessageTypesHttps.DiscoveryDiscloseMessageType : MessageTypes.DiscoveryDiscloseMessageType;
            Protocols = new List<DisclosedMessageProtocol>();
        }

        /// <inheritdoc />
        public DiscoveryDiscloseMessage(bool useMessageTypesHttps = false) : base(useMessageTypesHttps)
        {
            Id = Guid.NewGuid().ToString();
            Type = UseMessageTypesHttps ? MessageTypesHttps.DiscoveryDiscloseMessageType : MessageTypes.DiscoveryDiscloseMessageType;
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
