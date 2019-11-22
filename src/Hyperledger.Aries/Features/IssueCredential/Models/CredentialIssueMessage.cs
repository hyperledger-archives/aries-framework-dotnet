using System;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Decorators.Attachments;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.IssueCredential
{
    /// <summary>
    /// A credential content message.
    /// </summary>
    public class CredentialIssueMessage : AgentMessage
    {
        /// <inheritdoc />
        public CredentialIssueMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypes.IssueCredentialNames.IssueCredential;
        }

        /// <summary>
        /// Gets or sets human readable information about this Credential Proposal
        /// </summary>
        /// <value></value>
        [JsonProperty("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the offer attachment
        /// </summary>
        /// <value></value>
        [JsonProperty("credentials~attach")]
        public Attachment[] Credentials { get; set; }
    }
}