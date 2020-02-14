using System;
using Hyperledger.Aries.Features.IssueCredential;

namespace WebAgent.Models
{
    public class CredentialViewModel
    {
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }

        public CredentialState State { get; set; }
    }
}
