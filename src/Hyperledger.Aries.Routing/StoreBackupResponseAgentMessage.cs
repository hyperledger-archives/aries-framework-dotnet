using Hyperledger.Aries.Agents;
using System;

namespace Hyperledger.Aries.Routing
{
    public class StoreBackupResponseAgentMessage : AgentMessage
    {
        public StoreBackupResponseAgentMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = BackupTypeNames.StoreBackupResponseAgentMessage;
        }
        public DateTimeOffset BackupTimestamp { get; set; }
    }
}