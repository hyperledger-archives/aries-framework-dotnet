using Hyperledger.Aries.Agents;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Common
{
    /// <summary>
    ///  The base class for an ACK Message based on:
    ///  https://github.com/hyperledger/aries-rfcs/blob/main/features/0015-acks/README.md
    /// </summary>
    public abstract class AcknowledgeMessage : AgentMessage
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AcknowledgeMessage"/> class.
        /// </summary>
        protected AcknowledgeMessage(bool useMessageTypesHttps = false) : base(useMessageTypesHttps)
        {
        }
        
        /// <summary>
        /// Gets or sets the acknowledgement status.
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
