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
        public CredentialPreviewMessage() : base()
        {
            Id = null;
            Type = UseMessageTypesHttps ? MessageTypesHttps.IssueCredentialNames.PreviewCredential : MessageTypes.IssueCredentialNames.PreviewCredential;
        }

        /// <inheritdoc />
        public CredentialPreviewMessage(bool useMessageTypesHttps = false) : base(useMessageTypesHttps)
        {
            Id = null;
            Type = UseMessageTypesHttps ? MessageTypesHttps.IssueCredentialNames.PreviewCredential : MessageTypes.IssueCredentialNames.PreviewCredential;
        }

        /// <summary>
        /// Gets or sets the attributes
        /// </summary>
        /// <value></value>
        [JsonProperty("attributes")]
        public CredentialPreviewAttribute[] Attributes { get; set; }
    }
}
