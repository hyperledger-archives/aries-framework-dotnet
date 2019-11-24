using System.Collections.Generic;

namespace Hyperledger.Aries.Agents.Edge
{
    public class RestoreInboxMessage : AgentMessage
    {
        public string WalletId { get; set; }
        public string WalletKey { get; set; }

        public Dictionary<string, string> Metadata { get; set; }
    }
}