using Hyperledger.Aries.Agents;

namespace Hyperledger.Aries.Routing
{
    public class InboxItemMessage : AgentMessage
    {
        /// <summary>
        /// String representation of binary data in UTF-8 byte mark
        /// </summary>
        /// <value></value>
        public string Data { get; set; }

        /// <summary>
        /// Timestamp when this message was received
        /// </summary>
        public long Timestamp { get; set; }
    }
}