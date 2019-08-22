using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AgentFramework.Core.Messages.Discovery
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
        public string ProtocolId { get; set; }

        /// <summary>
        /// Roles for the subject protocol.
        /// </summary>
        public IList<string> Roles { get; set; }
    }
}
