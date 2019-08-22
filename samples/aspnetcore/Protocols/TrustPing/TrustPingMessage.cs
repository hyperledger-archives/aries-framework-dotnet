using System;
using AgentFramework.Core.Messages;
using Newtonsoft.Json;

namespace WebAgent.Messages
{
    /// <summary>
    /// A ping message.
    /// </summary>
    public class TrustPingMessage : AgentMessage
    {
        public TrustPingMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = CustomMessageTypes.TrustPingMessageType;
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
