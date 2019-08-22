using System;
using Newtonsoft.Json;

namespace AgentFramework.Core.Messages.Routing
{
    /// <summary>
    /// Represents a forwarding message
    /// </summary>
    public class ForwardMessage : AgentMessage
    {
        /// <inheritdoc />
        public ForwardMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypes.Forward;
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
        public string Message { get; set; }

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"Id={Id}, " +
            $"Type={Type}, " +
            $"To={To}, " +
            $"Message={(Message?.Length > 0 ? "[hidden]" : null)}";
    }
}
