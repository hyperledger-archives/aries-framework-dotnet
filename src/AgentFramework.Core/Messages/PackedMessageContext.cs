using AgentFramework.Core.Models.Records;

namespace AgentFramework.Core.Messages
{
    /// <summary>
    /// Represents an agent message in encrypted (packed) format
    /// </summary>
    public class PackedMessageContext : MessageContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PackedMessageContext" /> class.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public PackedMessageContext(byte[] message) : base(message, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackedMessageContext" /> class.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public PackedMessageContext(string message) : base(message, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackedMessageContext" /> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public PackedMessageContext(string message, ConnectionRecord connection) : base(message, true, connection)
        {
        }
    }
}
