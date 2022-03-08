using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.Handshakes.Common;
using Newtonsoft.Json.Linq;

namespace Hyperledger.Aries.Agents
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
        public PackedMessageContext(JObject message) : base(message.ToJson(), true)
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
