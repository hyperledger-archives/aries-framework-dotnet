using Hyperledger.Aries.Agents;

namespace Hyperledger.Aries.Routing
{
    public class ListBackupsAgentMessage : AgentMessage
    {
        public string BackupId { get; set; }
    }
}