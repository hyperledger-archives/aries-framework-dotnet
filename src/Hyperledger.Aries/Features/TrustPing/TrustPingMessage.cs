using System;
using System.Text.Json.Serialization;
using Hyperledger.Aries.Agents;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.TrustPing
{
    /// <summary>
    /// A ping message.
    /// </summary>
    public class TrustPingMessage : AgentMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AgentFramework.Core.Messages.Common.TrustPingMessage"/> class.
        /// </summary>
        public TrustPingMessage() : base()
        {
            Id = Guid.NewGuid().ToString();
            Type = UseMessageTypesHttps ? MessageTypesHttps.TrustPingMessageType : MessageTypes.TrustPingMessageType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AgentFramework.Core.Messages.Common.TrustPingMessage"/> class.
        /// </summary>
        public TrustPingMessage(bool useMessageTypesHttps = false) : base(useMessageTypesHttps)
        {
            Id = Guid.NewGuid().ToString();
            Type = UseMessageTypesHttps ? MessageTypesHttps.TrustPingMessageType : MessageTypes.TrustPingMessageType;
        }

        /// <summary>
        /// Gets or sets the comment of the message.
        /// </summary>
        /// <value>
        /// The comment.
        /// </value>
        [JsonProperty("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the comment of the message.
        /// </summary>
        /// <value>
        /// The comment.
        /// </value>
        [JsonProperty("response_requested")]
        [JsonPropertyName("response_requested")]
        public bool ResponseRequested { get; set; }
    }
}
