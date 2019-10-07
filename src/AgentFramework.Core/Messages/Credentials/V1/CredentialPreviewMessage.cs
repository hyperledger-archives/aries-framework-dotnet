using System;
using Newtonsoft.Json;

namespace AgentFramework.Core.Messages.Credentials.V1
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
        public CredentialPreviewAttriubute[] Attributes { get; set; }
    }

    /// <summary>
    /// Credential Preview Attribute
    /// </summary>
    public class CredentialPreviewAttriubute
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        /// <value></value>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the mime type
        /// </summary>
        /// <value></value>
        [JsonProperty("mime-type")]
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        /// <value></value>
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}