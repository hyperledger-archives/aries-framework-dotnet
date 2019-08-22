using System;
using Newtonsoft.Json;

namespace AgentFramework.Core.Messages.Common
{
    /// <summary>
    /// Basic message.
    /// </summary>
    public class BasicMessage : AgentMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AgentFramework.Core.Messages.Common.BasicMessage"/> class.
        /// </summary>
        public BasicMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypes.BasicMessageType;
        }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        [JsonProperty("content")]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the UTC sent time in ISO 8601 string format
        /// </summary>
        /// <value>The sent time.</value>
        [JsonProperty("sent_time")]
        public string SentTime { get; set; }
    }
}
