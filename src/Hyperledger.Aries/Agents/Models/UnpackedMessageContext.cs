using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.Handshakes.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hyperledger.Aries.Agents
{
    /// <summary>
    /// Represents an agent message in unencrypted (unpacked) format
    /// </summary>
    public class UnpackedMessageContext : MessageContext
    { 
        /// <summary>
        /// Initializes a new instance of the <see cref="UnpackedMessageContext" /> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="senderVerkey"></param>
        /// <returns></returns>
        public UnpackedMessageContext(AgentMessage message, string senderVerkey) : base(message)
        {
            SenderVerkey = senderVerkey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnpackedMessageContext" /> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public UnpackedMessageContext(AgentMessage message, ConnectionRecord connection) : base(message, connection)
        {
            SenderVerkey = connection.TheirVk;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnpackedMessageContext" /> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public UnpackedMessageContext(string message, ConnectionRecord connection) : base(message, false, connection)
        {
            SenderVerkey = connection.TheirVk;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnpackedMessageContext" /> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="senderVerkey"></param>
        /// <returns></returns>
        public UnpackedMessageContext(string message, string senderVerkey) : base(message, false)
        {
            SenderVerkey = senderVerkey;
        }

        /// <summary>
        /// The verification key of the sender. Can be null.
        /// </summary>
        /// <value></value>
        public string SenderVerkey { get; }


        /// <summary>
        /// The message id of the current message.
        /// </summary>
        public string GetMessageId() =>
            Packed
                ? throw new AriesFrameworkException(ErrorCode.InvalidMessage, "Cannot deserialize packed message.")
                : MessageJson["@id"].Value<string>();

        /// <summary>
        /// The message type of the current message.
        /// </summary>
        public string GetMessageType() =>
            Packed
                ? throw new AriesFrameworkException(ErrorCode.InvalidMessage, "Cannot deserialize packed message.")
                : MessageJson["@type"].Value<string>();

        /// <summary>
        /// Gets the message cast to the expect message type.
        /// </summary>
        /// <typeparam name="T">The generic type the message will be cast to.</typeparam>
        /// <returns>The agent message.</returns>
        public T GetMessage<T>() where T : AgentMessage, new() =>
            Packed
                ? throw new AriesFrameworkException(ErrorCode.InvalidMessage, "Cannot deserialize packed message.")
                : JsonConvert.DeserializeObject<T>(MessageJson.ToString(), new AgentMessageReader<T>());

        /// <summary>
        /// Gets the message cast to the expect message type.
        /// </summary>
        /// <returns>The agent message.</returns>
        public string GetMessageJson() =>
            Packed
                ? throw new AriesFrameworkException(ErrorCode.InvalidMessage, "Cannot deserialize packed message.")
                : MessageJson.ToJson();
    }
}
