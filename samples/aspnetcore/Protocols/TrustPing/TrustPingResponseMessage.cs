using System;
using AgentFramework.Core.Messages;
using Newtonsoft.Json;

namespace WebAgent.Messages
{
    /// <summary>
    /// A ping response message.
    /// </summary>
    public class TrustPingResponseMessage : AgentMessage
    {
        public TrustPingResponseMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = CustomMessageTypes.TrustPingResponseMessageType;
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
