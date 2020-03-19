using Hyperledger.Aries.Agents;
using System;

namespace Hyperledger.Aries.Routing
{
    public class RetrieveBackupAgentMessage : AgentMessage
    {
        public RetrieveBackupAgentMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = BackupTypeNames.RetrieveBackupAgentMessage;
        }

        public string BackupId { get; set; }
        public string Signature { get; set; }
    }
}