using System;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Decorators.Signature;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.DidExchange
{
    /// <summary>
    /// Represents a connection response message
    /// </summary>
    public class ConnectionResponseMessage : AgentMessage
    {
        /// <inheritdoc />
        public ConnectionResponseMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypes.ConnectionResponse;
        }
        
        /// <summary>
        /// Gets or sets the connection object.
        /// </summary>
        /// <value>
        /// The connection object.
        /// </value>
        [JsonProperty("connection~sig")]
        public SignatureDecorator ConnectionSig { get; set; }
    }
}
