using System;
using Hyperledger.Aries.Agents;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.IssueCredential.Models
{
    /// <summary>
    /// Credential acknowledge message
    /// </summary>
    public class CredentialAcknowledgeMessage : AgentMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialAcknowledgeMessage" /> class.
        /// </summary>
        public CredentialAcknowledgeMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = UseMessageTypesHttps ? MessageTypesHttps.IssueCredentialNames.AcknowledgeCredential : MessageTypes.IssueCredentialNames.AcknowledgeCredential;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialAcknowledgeMessage" /> class.
        /// </summary>
        public CredentialAcknowledgeMessage(bool useMessageTypesHttps = false) : base(useMessageTypesHttps)
        {
            Id = Guid.NewGuid().ToString();
            Type = UseMessageTypesHttps ? MessageTypesHttps.IssueCredentialNames.AcknowledgeCredential : MessageTypes.IssueCredentialNames.AcknowledgeCredential;
        }
        
        /// <summary>
        /// Gets or sets the acknowledgement status.
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
