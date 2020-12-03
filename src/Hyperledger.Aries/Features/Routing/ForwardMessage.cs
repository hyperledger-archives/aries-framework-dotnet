using System;
using System.Text.Json.Serialization;
using Hyperledger.Aries.Agents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hyperledger.Aries.Features.Routing
{
    /// <summary>
    /// Represents a forwarding message
    /// </summary>
    public class ForwardMessage : AgentMessage
    {
        /// <inheritdoc />
        public ForwardMessage() : base()
        {
            Id = Guid.NewGuid().ToString();
            Type = UseMessageTypesHttps ? MessageTypesHttps.Forward : MessageTypes.Forward;
        }

        /// <inheritdoc />
        public ForwardMessage(bool useMessageTypesHttps = false) : base(useMessageTypesHttps)
        {
            Id = Guid.NewGuid().ToString();
            Type = UseMessageTypesHttps ? MessageTypesHttps.Forward : MessageTypes.Forward;
        }

        /// <summary>
        /// Gets or sets the to or recipient field.
        /// </summary>
        /// <value>
        /// The to or recipient of the message.
        /// </value>
        [JsonProperty("to")]
        public string To { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        [JsonProperty("msg")]
        [JsonPropertyName("msg")]
        public JObject Message { get; set; }
    }
}
