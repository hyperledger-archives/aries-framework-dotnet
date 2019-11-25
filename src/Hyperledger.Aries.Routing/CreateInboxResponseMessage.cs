using Hyperledger.Aries.Agents;

namespace Hyperledger.Aries.Routing
{
    public class CreateInboxResponseMessage : AgentMessage
    {
        public string InboxId { get; set; }

        public string InboxKey { get; set; }
    }
}