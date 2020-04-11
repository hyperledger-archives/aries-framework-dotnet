using System;
using Hyperledger.Aries.Agents;

namespace Hyperledger.Aries.Routing
{
    public class ListBackupsAgentMessage : AgentMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListBackupsAgentMessage"/> class.
        /// </summary>
        public ListBackupsAgentMessage()
        {
            Id = new Guid().ToString();
            Type = BackupTypeNames.ListBackupsAgentMessage;
        }
        /// <summary>
        /// Gets or sets the backup identifier.
        /// </summary>
        /// <value>
        /// The backup identifier.
        /// </value>
        public string BackupId { get; set; }
    }
}