using System;
using System.Text.Json.Serialization;
using Hyperledger.Aries.Agents;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.BasicMessage
{
    /// <summary>
    /// Basic message.
    /// </summary>
    public class BasicMessage : AgentMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AgentFramework.Core.Messages.Common.BasicMessage"/> class.
        /// </summary>
        public BasicMessage() : base()
        {
            Id = Guid.NewGuid().ToString();
            Type = UseMessageTypesHttps ? MessageTypesHttps.BasicMessageType : MessageTypes.BasicMessageType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AgentFramework.Core.Messages.Common.BasicMessage"/> class.
        /// </summary>
        public BasicMessage(bool useMessageTypesHttps = false) : base (useMessageTypesHttps)
        {
            Id = Guid.NewGuid().ToString();
            Type = UseMessageTypesHttps ? MessageTypesHttps.BasicMessageType : MessageTypes.BasicMessageType;
        }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        [JsonProperty("content")]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the UTC sent time in ISO 8601 string format.
        /// Use <see cref="DateTime.ToString(string, IFormatProvider)" />
        /// with format "u" - Universal Sortable Time
        /// </summary>
        /// <value>The sent time.</value>
        [JsonProperty("sent_time")]
        [JsonPropertyName("sent_time")]
        public string SentTime { get; set; }
    }
}
