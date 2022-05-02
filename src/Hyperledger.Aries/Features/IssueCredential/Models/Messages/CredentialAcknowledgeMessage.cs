using System;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Common;

namespace Hyperledger.Aries.Features.IssueCredential.Models.Messages
{
    /// <summary>
    /// Credential acknowledge message
    /// </summary>
    public class CredentialAcknowledgeMessage : AcknowledgeMessage
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
    }
}
