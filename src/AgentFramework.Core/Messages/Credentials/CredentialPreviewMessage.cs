using System;
using AgentFramework.Core.Models.Credentials;
using Newtonsoft.Json;

namespace AgentFramework.Core.Messages
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