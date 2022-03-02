using System;
using System.Text.Json.Serialization;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Decorators.Signature;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.Handshakes.Connection.Models
{
    /// <summary>
    /// Represents a connection response message
    /// </summary>
    public class ConnectionResponseMessage : AgentMessage
    {
        /// <inheritdoc />
        public ConnectionResponseMessage() : base()
        {
            Id = Guid.NewGuid().ToString();
            Type = UseMessageTypesHttps ? MessageTypesHttps.ConnectionResponse : MessageTypes.ConnectionResponse;
        }

        /// <inheritdoc />
        public ConnectionResponseMessage(bool useMessageTypesHttps = false) : base(useMessageTypesHttps)
        {
            Id = Guid.NewGuid().ToString();
            Type = UseMessageTypesHttps ? MessageTypesHttps.ConnectionResponse : MessageTypes.ConnectionResponse;
        }
        
        /// <summary>
        /// Gets or sets the connection object.
        /// </summary>
        /// <value>
        /// The connection object.
        /// </value>
        [JsonProperty("connection~sig")]
        [JsonPropertyName("connection~sig")]
        public SignatureDecorator ConnectionSig { get; set; }
    }
}
