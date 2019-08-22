using System;
using Newtonsoft.Json;

namespace AgentFramework.Core.Messages.Common
{
    /// <summary>
    /// A ping message.
    /// </summary>
    public class TrustPingMessage : AgentMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AgentFramework.Core.Messages.Common.TrustPingMessage"/> class.
        /// </summary>
        public TrustPingMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypes.TrustPingMessageType;
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
        public bool ResponseRequested { get; set; }
    }
}
