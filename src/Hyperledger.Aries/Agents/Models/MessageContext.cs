using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.Handshakes.Common;
using Hyperledger.Aries.Storage;
using Newtonsoft.Json.Linq;

namespace Hyperledger.Aries.Agents
{
    /// <summary>
    /// A message context object that surrounds an agent message
    /// </summary>
    public abstract class MessageContext
    {
        /// <summary>
        /// The message as JSON
        /// </summary>
        protected readonly JObject MessageJson;

        /// <summary>Initializes a new instance of the <see cref="MessageContext"/> class.</summary>
        /// <param name="message">The message.</param>
        /// <param name="packed">if set to <c>true</c> [packed].</param>
        protected MessageContext(byte[] message, bool packed)
        {
            Packed = packed;
            Payload = message;
            if (!Packed)
            {
                MessageJson = JObject.Parse(message.GetUTF8String());
            }
        }

        /// <inheritdoc />
        protected MessageContext(string message, bool packed) : this(message.GetUTF8Bytes(), packed)
        {
        }

        /// <inheritdoc />
        protected MessageContext(string message, bool packed, ConnectionRecord connection) : this(message.GetUTF8Bytes(), packed)
        {
            Connection = connection;
        }

        /// <inheritdoc />
        /// <param name="message">The message.</param>
        protected MessageContext(AgentMessage message)
        : this(message.ToJson(), false)
        { }

        /// <inheritdoc />
        /// <param name="message">The message.</param>
        /// <param name="connection">The connection.</param>
        protected MessageContext(AgentMessage message, ConnectionRecord connection)
        : this(message.ToJson(), false)
        {
            Connection = connection;
        }

        /// <summary>
        /// The raw format of the message.
        /// </summary>
        public byte[] Payload { get; }

        /// <summary>Gets a value indicating whether this <see cref="MessageContext"/> is packed.</summary>
        /// <value>
        ///   <c>true</c> if packed; otherwise, <c>false</c>.</value>
        public bool Packed { get; }

        /// <summary>
        /// Gets the connection associated to the message.
        /// </summary>
        /// <returns>The associated connection to the message.</returns>
        public ConnectionRecord Connection { get; set; }

        /// <summary>
        /// Gets the record associated with this message context.
        /// May be <code>null</code>.
        /// </summary>
        /// <value>The context record.</value>
        public RecordBase ContextRecord { get; set; }
    }
}
