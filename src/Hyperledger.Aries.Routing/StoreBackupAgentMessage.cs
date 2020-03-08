using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Decorators.Attachments;
using System;
using System.Collections.Generic;

namespace Hyperledger.Aries.Routing
{
    public class StoreBackupAgentMessage : AgentMessage
    {
        public StoreBackupAgentMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = BackupTypeNames.StoreBackupAgentMessage;
        }

        public string BackupId { get; set; }
        public List<Attachment> Payload { get; set; }
        public string PayloadSignature { get; set; }
    }
}