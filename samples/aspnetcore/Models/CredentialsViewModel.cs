using System.Collections.Generic;
using AgentFramework.Core.Models.Records;
using WebAgent.Protocols;
using WebAgent.Protocols.BasicMessage;

namespace WebAgent.Models
{
    public class CredentialsViewModel
    {
        public IEnumerable<CredentialViewModel> Credentials { get; set; }
    }
}
