using System;
using Newtonsoft.Json;
// ReSharper disable VirtualMemberCallInConstructor

namespace AgentFramework.Core.Models.Records
{
    /// <summary>
    /// Represents a private message record in the user's wallet
    /// </summary>
    /// <seealso cref="AgentFramework.Core.Models.Records.RecordBase" />
    public class BasicMessageRecord : RecordBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicMessageRecord"/> class.
        /// </summary>
        public BasicMessageRecord()
        {
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        public override string TypeName => "WebAgent.BasicMessage";

        /// <summary>
        /// Gets or sets the connection identifier.
        /// </summary>
        /// <value>
        /// The connection identifier.
        /// </value>
        [JsonIgnore]
        public string ConnectionId
        {
            get => Get();
            set => Set(value);
        }

        /// <summary>
        /// Gets or sets the sent time.
        /// </summary>
        /// <value>
        /// The sent time.
        /// </value>
        public DateTime SentTime { get; set; }

        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        /// <value>
        /// The direction.
        /// </value>
        public MessageDirection Direction { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text { get; set; }
    }

    /// <summary>
    /// Message direction
    /// </summary>
    public enum MessageDirection
    {
        /// <summary>
        /// The incoming
        /// </summary>
        Incoming,
        /// <summary>
        /// The outgoing
        /// </summary>
        Outgoing
    }
}
