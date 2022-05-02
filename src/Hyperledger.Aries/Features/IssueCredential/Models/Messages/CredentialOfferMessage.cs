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
    public class CredentialOfferMessage : AgentMessage
    {
        /// <inheritdoc />
        public CredentialOfferMessage() : base()
        {
            Id = Guid.NewGuid().ToString();
            Type = UseMessageTypesHttps ? MessageTypesHttps.IssueCredentialNames.OfferCredential : MessageTypes.IssueCredentialNames.OfferCredential;
        }

        /// <inheritdoc />
        public CredentialOfferMessage(bool useMessageTypesHttps = false) : base(useMessageTypesHttps)
        {
            Id = Guid.NewGuid().ToString();
            Type = UseMessageTypesHttps ? MessageTypesHttps.IssueCredentialNames.OfferCredential : MessageTypes.IssueCredentialNames.OfferCredential;
        }

        /// <summary>
        /// Gets or sets human readable information about this Credential Proposal
        /// </summary>
        /// <value></value>
        [JsonProperty("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the Credential Preview
        /// </summary>
        /// <value></value>
        [JsonProperty("credential_preview")]
        [JsonPropertyName("credential_preview")]
        public CredentialPreviewMessage CredentialPreview { get; set; }

        /// <summary>
        /// Gets or sets the offer attachment
        /// </summary>
        /// <value></value>
        [JsonProperty("offers~attach")]
        [JsonPropertyName("offers~attach")]
        public Attachment[] Offers { get; set; }
    }
}
