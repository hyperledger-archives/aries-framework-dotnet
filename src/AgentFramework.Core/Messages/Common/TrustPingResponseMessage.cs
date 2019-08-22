using System;
using Newtonsoft.Json;

namespace AgentFramework.Core.Messages.Common
{
    /// <summary>
    /// A ping response message.
    /// </summary>
    public class TrustPingResponseMessage : AgentMessage
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:AgentFramework.Core.Messages.Common.TrustPingResponseMessage"/> class.
        /// </summary>
        public TrustPingResponseMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypes.TrustPingResponseMessageType;
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