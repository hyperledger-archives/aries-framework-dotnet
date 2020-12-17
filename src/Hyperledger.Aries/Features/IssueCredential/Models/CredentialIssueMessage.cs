using System;
using System.Text.Json.Serialization;
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
        public CredentialIssueMessage() : base()
        {
            Id = Guid.NewGuid().ToString();
            Type = UseMessageTypesHttps ? MessageTypesHttps.IssueCredentialNames.IssueCredential : MessageTypes.IssueCredentialNames.IssueCredential;
        }

        /// <inheritdoc />
        public CredentialIssueMessage(bool useMessageTypesHttps = false) : base(useMessageTypesHttps)
        {
            Id = Guid.NewGuid().ToString();
            Type = UseMessageTypesHttps ? MessageTypesHttps.IssueCredentialNames.IssueCredential : MessageTypes.IssueCredentialNames.IssueCredential;
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
        [JsonPropertyName("credentials~attach")]
        public Attachment[] Credentials { get; set; }
    }
}
