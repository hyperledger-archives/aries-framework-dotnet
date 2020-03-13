using Hyperledger.Aries.Agents;
using System;
using System.Collections.Generic;

namespace Hyperledger.Aries.Routing
{
    public class ListBackupsResponseAgentMessage : AgentMessage
    {
        public ListBackupsResponseAgentMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = BackupTypeNames.ListBackupsResponseAgentMessage;
        }

        public IEnumerable<string> BackupList { get; set; }
    }
}