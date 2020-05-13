using Hyperledger.Aries.Agents;

namespace Hyperledger.Aries.Routing
{
    public class CreateInboxResponseMessage : AgentMessage
    {
        /// <summary>
        /// Gets or sets the inbox identifier.
        /// </summary>
        /// <value>
        /// The inbox identifier.
        /// </value>
        public string InboxId { get; set; }

        /// <summary>
        /// Gets or sets the inbox key.
        /// </summary>
        /// <value>
        /// The inbox key.
        /// </value>
        public string InboxKey { get; set; }
    }
}