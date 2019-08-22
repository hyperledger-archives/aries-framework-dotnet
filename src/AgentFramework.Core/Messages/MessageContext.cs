using System;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Models.Records;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AgentFramework.Core.Messages
{
    /// <summary>
    /// A message context object that surrounds an agent message
    /// </summary>
    public sealed class MessageContext
    {
        private readonly JObject _messageJson;

        internal bool HasDecorator(string v)
        {
            throw new NotImplementedException();
        }

        /// <summary>Initializes a new instance of the <see cref="MessageContext"/> class.</summary>
        /// <param name="message">The message.</param>
        /// <param name="packed">if set to <c>true</c> [packed].</param>
        public MessageContext(byte[] message, bool packed)
        {
            Packed = packed;
            Payload = message;
            if (!Packed) _messageJson = JObject.Parse(Payload.GetUTF8String());
        }

        /// <summary>Initializes a new instance of the <see cref="MessageContext"/> class.</summary>
        /// <param name="message">The message.</param>
        /// <param name="packed">if set to <c>true</c> [packed].</param>
        /// <param name="connection">The connection.</param>
        public MessageContext(byte[] message, bool packed, ConnectionRecord connection)
        {
            Packed = packed;
            Payload = message;
            if (!Packed) _messageJson = JObject.Parse(Payload.GetUTF8String());
        }

        /// <inheritdoc />
        public MessageContext(string message, bool packed) : this(message.GetUTF8Bytes(), packed)
        {
        }

        /// <inheritdoc />
        public MessageContext(string message, bool packed, ConnectionRecord connection) : this(message.GetUTF8Bytes(), packed)
        {
            Connection = connection;
        }

        /// <inheritdoc />
        /// <param name="message">The message.</param>
        public MessageContext(AgentMessage message)
        : this(message.ToJson(), false)
        { }

        /// <inheritdoc />
        /// <param name="message">The message.</param>
        /// <param name="connection">The connection.</param>
        public MessageContext(AgentMessage message, ConnectionRecord connection)
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
        public ConnectionRecord Connection { get; }

        /// <summary>
        /// Gets the record associated with this message context.
        /// May be <code>null</code>.
        /// </summary>
        /// <value>The context record.</value>
        public RecordBase ContextRecord { get; set; }

        /// <summary>
        /// The message id of the current message.
        /// </summary>
        public string GetMessageId() =>
            Packed
                ? throw new AgentFrameworkException(ErrorCode.InvalidMessage, "Cannot deserialize packed message.")
                : _messageJson["@id"].Value<string>();

        /// <summary>
        /// The message type of the current message.
        /// </summary>
        public string GetMessageType() =>
            Packed
                ? throw new AgentFrameworkException(ErrorCode.InvalidMessage, "Cannot deserialize packed message.")
                : _messageJson["@type"].Value<string>();

        /// <summary>
        /// Gets the message cast to the expect message type.
        /// </summary>
        /// <typeparam name="T">The generic type the message will be cast to.</typeparam>
        /// <returns>The agent message.</returns>
        public T GetMessage<T>() where T : AgentMessage, new() =>
            Packed
                ? throw new AgentFrameworkException(ErrorCode.InvalidMessage, "Cannot deserialize packed message.")
                : JsonConvert.DeserializeObject<T>(_messageJson.ToString(), new AgentMessageReader<T>());

        /// <summary>
        /// Gets the message cast to the expect message type.
        /// </summary>
        /// <returns>The agent message.</returns>
        public string GetMessageJson() =>
            Packed
                ? throw new AgentFrameworkException(ErrorCode.InvalidMessage, "Cannot deserialize packed message.")
                : _messageJson.ToJson();
    }
}
