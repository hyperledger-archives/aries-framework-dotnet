using System;
using AgentFramework.Core.Models.Records;

namespace WebAgent.Models
{
    public class CredentialViewModel
    {
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }

        public CredentialState State { get; set; }
    }
}
