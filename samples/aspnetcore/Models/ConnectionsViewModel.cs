using System.Collections.Generic;
using AgentFramework.Core.Models.Records;
using WebAgent.Protocols;
using WebAgent.Protocols.BasicMessage;

namespace WebAgent.Models
{
    public class ConnectionsViewModel
    {
        public IEnumerable<ConnectionRecord> Connections { get; set; }

        public IEnumerable<ConnectionRecord> Invitations { get; set; }
    }

    public class ConnectionDetailsViewModel
    {
        public ConnectionRecord Connection { get; set; }

        public IEnumerable<BasicMessageRecord> Messages { get; set; }

        public bool? TrustPingSuccess { get; set; }
    }
}
