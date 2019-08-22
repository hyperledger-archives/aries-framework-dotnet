using System;
using AgentFramework.Core.Decorators.Signature;
using Newtonsoft.Json;

namespace AgentFramework.Core.Messages.Connections
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

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"Id={Id}, " +
            $"Type={Type}, ";
    }
}
