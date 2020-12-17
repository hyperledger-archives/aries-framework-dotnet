using System;
using System.Text.Json.Serialization;
using Hyperledger.Aries.Agents;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.IssueCredential
{
    /// <summary>
    /// A credential content message.
    /// </summary>
    public class CredentialProposeMessage : AgentMessage
    {
        /// <inheritdoc />
        public CredentialProposeMessage() : base()
        {
            Id = Guid.NewGuid().ToString();
            Type = UseMessageTypesHttps ? MessageTypesHttps.IssueCredentialNames.ProposeCredential : MessageTypes.IssueCredentialNames.ProposeCredential;
        }

        /// <inheritdoc />
        public CredentialProposeMessage(bool useMessageTypesHttps = false) : base(useMessageTypesHttps)
        {
            Id = Guid.NewGuid().ToString();
            Type = UseMessageTypesHttps ? MessageTypesHttps.IssueCredentialNames.ProposeCredential : MessageTypes.IssueCredentialNames.ProposeCredential;
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
        [JsonPropertyName("schema_id")]
        public string SchemaId { get; set; }

        /// <summary>
        /// Gets or sets the credential definition identifier
        /// </summary>
        /// <value></value>
        [JsonProperty("cred_def_id")]
        [JsonPropertyName("cred_def_id")]
        public string CredentialDefinitionId { get; set; }
    }
}
