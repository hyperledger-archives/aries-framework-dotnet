using System;
using Hyperledger.Aries.Agents;

namespace Hyperledger.Aries.Routing
{
    public class ListBackupsAgentMessage : AgentMessage
    {
        public ListBackupsAgentMessage()
        {
            Id = new Guid().ToString();
            Type = BackupTypeNames.ListBackupsAgentMessage;
        }
        public string BackupId { get; set; }
    }
}