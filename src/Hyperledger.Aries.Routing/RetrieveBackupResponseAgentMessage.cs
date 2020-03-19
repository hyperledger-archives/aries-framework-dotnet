using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Decorators.Attachments;
using System;
using System.Collections.Generic;

namespace Hyperledger.Aries.Routing
{
    public class RetrieveBackupResponseAgentMessage : AgentMessage
    {
        public RetrieveBackupResponseAgentMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = BackupTypeNames.RetrieveBackupResponseAgentMessage;
        }

        public List<Attachment> Payload { get; set; }
    }
}