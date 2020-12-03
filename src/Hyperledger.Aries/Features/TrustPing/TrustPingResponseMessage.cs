using System;
using Hyperledger.Aries.Agents;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.TrustPing
{
    /// <summary>
    /// A ping response message.
    /// </summary>
    public class TrustPingResponseMessage : AgentMessage
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TrustPingResponseMessage"/> class.
        /// </summary>
        public TrustPingResponseMessage() : base()
        {
            Id = Guid.NewGuid().ToString();
            Type = UseMessageTypesHttps ? MessageTypesHttps.TrustPingResponseMessageType : MessageTypes.TrustPingResponseMessageType;
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TrustPingResponseMessage"/> class.
        /// </summary>
        public TrustPingResponseMessage(bool useMessageTypesHttps = false) : base(useMessageTypesHttps)
        {
            Id = Guid.NewGuid().ToString();
            Type = UseMessageTypesHttps ? MessageTypesHttps.TrustPingResponseMessageType : MessageTypes.TrustPingResponseMessageType;
        }

        /// <summary>
        /// Gets or sets the comment of the message.
        /// </summary>
        /// <value>
        /// The comment.
        /// </value>
        [JsonProperty("comment")]
        public string Comment { get; set; }
    }
}
