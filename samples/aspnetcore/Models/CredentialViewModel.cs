using System;
using System.Collections.Generic;
using Hyperledger.Aries.Features.IssueCredential;

namespace WebAgent.Models
{
    public class CredentialViewModel
    {
        public string SchemaId { get; set; }

        public DateTime CreatedAt { get; set; }

        public CredentialState State { get; set; }

        public IEnumerable<CredentialPreviewAttribute> CredentialAttributesValues { get; set; }
    }
}
