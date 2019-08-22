using System;
using AgentFramework.Core.Models.Records;
using Newtonsoft.Json;

namespace WebAgent.Protocols.BasicMessage
{
    /// <summary>
    /// Represents a private message record in the user's wallet
    /// </summary>
    /// <seealso cref="AgentFramework.Core.Models.Records.RecordBase" />
    public class BasicMessageRecord : RecordBase
    {
        public override string TypeName => "WebAgent.BasicMessage";

        [JsonIgnore]
        public string ConnectionId
        {
            get => Get();
            set => Set(value);
        }

        public DateTime SentTime { get; set; }

        public MessageDirection Direction { get; set; }

        public string Text { get; set; }
    }

    public enum MessageDirection
    {
        Incoming,
        Outgoing
    }
}