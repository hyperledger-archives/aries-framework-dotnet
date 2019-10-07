using System;
using Newtonsoft.Json;

namespace AgentFramework.Core.Messages.Credentials.V1
{
    /// <summary>
    /// A credential content message.
    /// </summary>
    public class CredentialProposeMessage : AgentMessage
    {
        /// <inheritdoc />
        public CredentialProposeMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypes.IssueCredentialNames.ProposeCredential;
        }

        /// <summary>
        /// Gets or sets human readable information about this Credential Proposal
        /// </summary>
        /// <value></value>
        [JsonProperty("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the schema identifier
        /// </summary>
        /// <value></value>
        [JsonProperty("schema_id")]
        public string SchemaId { get; set; }

        /// <summary>
        /// Gets or sets the credential definition identifier
        /// </summary>
        /// <value></value>
        [JsonProperty("cred_def_id")]
        public string CredentialDefinitionId { get; set; }
    }
}