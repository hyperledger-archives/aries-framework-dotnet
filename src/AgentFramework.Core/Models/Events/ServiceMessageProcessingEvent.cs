namespace AgentFramework.Core.Models.Events
{
    /// <summary>
    /// Representation of a message processing event.
    /// </summary>
    public class ServiceMessageProcessingEvent
    {
        /// <summary>
        /// Id of the thread the message is apart of.
        /// </summary>
        public string ThreadId { get; set; }

        /// <summary>
        /// Id of the effected record in persited state, if applicable.
        /// </summary>
        public string RecordId { get; set; }

        /// <summary>
        /// Agent Message Type.
        /// </summary>
        public string MessageType { get; set; }
        
        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"ThreadId={ThreadId}, " +
            $"RecordId={RecordId}, " +
            $"MessageType={MessageType}";
    }
}
