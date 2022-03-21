using System;
using Hyperledger.Aries.Agents;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.Handshakes.Connection.Models
{
    /// <summary>
    /// Represents a connection request message.
    /// </summary>
    public class ConnectionRequestMessage : AgentMessage
    {
        /// <inheritdoc />
        public ConnectionRequestMessage() : base()
        {
            Id = Guid.NewGuid().ToString();
            Type = UseMessageTypesHttps ? MessageTypesHttps.ConnectionRequest : MessageTypes.ConnectionRequest;
        }

        /// <inheritdoc />
        public ConnectionRequestMessage(bool useMessageTypesHttps = false) : base(useMessageTypesHttps)
        {
            Id = Guid.NewGuid().ToString();
            Type = UseMessageTypesHttps ? MessageTypesHttps.ConnectionRequest : MessageTypes.ConnectionRequest;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [JsonProperty("label")]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the image URL.
        /// </summary>
        /// <value>
        /// The image URL.
        /// </value>
        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the connection object.
        /// </summary>
        /// <value>
        /// The connection object.
        /// </value>
        [JsonProperty("connection")]
        public Common.Connection Connection { get; set; }

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"Id={Id}, " +
            $"Type={Type}, " +
            $"Did={Connection?.Did}, " +
            $"Name={Label}, " +
            $"ImageUrl={ImageUrl}, ";
    }
}
