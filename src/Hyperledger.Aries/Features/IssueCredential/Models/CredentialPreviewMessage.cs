using Hyperledger.Aries.Agents;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.IssueCredential
{
    /// <summary>
    /// A credential content message.
    /// </summary>
    public class CredentialPreviewMessage : AgentMessage
    {
        /// <inheritdoc />
        public CredentialPreviewMessage()
        {
            Id = null;
            Type = MessageTypes.IssueCredentialNames.PreviewCredential;
        }

        /// <summary>
        /// Gets or sets the attributes
        /// </summary>
        /// <value></value>
        [JsonProperty("attributes")]
        public CredentialPreviewAttribute[] Attributes { get; set; }
    }
}