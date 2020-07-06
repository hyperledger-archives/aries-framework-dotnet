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
    public class CredentialRequestMessage : AgentMessage
    {
        /// <inheritdoc />
        public CredentialRequestMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypes.IssueCredentialNames.RequestCredential;
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
        [JsonProperty("requests~attach")]
        [JsonPropertyName("requests~attach")]
        public Attachment[] Requests { get; set; }
    }
}