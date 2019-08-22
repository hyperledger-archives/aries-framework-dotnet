using System.Collections.Generic;
using Newtonsoft.Json;

namespace AgentFramework.Core.Models.Credentials
{
    /// <summary>
    /// Represents a credential stored in the wallet.
    /// </summary>
    public struct CredentialInfo
    {
        /// <summary>
        /// Gets or sets the referent (the credential id in the wallet).
        /// </summary>
        /// <value>
        /// The referent.
        /// </value>
        [JsonProperty("referent")]
        public string Referent { get; set; }

        /// <summary>
        /// Gets or sets the attributes and their raw values.
        /// </summary>
        /// <value>
        /// The attributes.
        /// </value>
        [JsonProperty("attrs")]
        public Dictionary<string, string> Attributes { get; set; }

        /// <summary>
        /// Gets or sets the schema identifier associated with this credential.
        /// </summary>
        /// <value>
        /// The schema identifier.
        /// </value>
        [JsonProperty("schema_id")]
        public string SchemaId { get; set; }

        /// <summary>
        /// Gets or sets the credential definition identifier associated with this credential.
        /// </summary>
        /// <value>
        /// The credential definition identifier.
        /// </value>
        [JsonProperty("cred_def_id")]
        public string CredentialDefinitionId { get; set; }

        /// <summary>
        /// Gets or sets the revocation registry identifier if supported by the definition, otherwise <c>null</c>.
        /// </summary>
        /// <value>
        /// The revocation registry identifier.
        /// </value>
        [JsonProperty("rev_reg_id")]
        public string RevocationRegistryId { get; set; }

        /// <summary>
        /// Gets or sets the credential revocation identifier if supported by the definition, otherwise <c>null</c>.
        /// </summary>
        /// <value>
        /// The credential revocation identifier.
        /// </value>
        [JsonProperty("cred_rev_id")]
        public string CredentialRevocationId { get; set; }
        
        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"Referent={Referent}, " +
            $"SchemaId={SchemaId}, " +
            $"CredentialDefinitionId={CredentialDefinitionId}, " +
            $"RevocationRegistryId={RevocationRegistryId}, " +
            $"CredentialRevocationId={CredentialRevocationId}, " +
            $"Attributes={string.Join(",", Attributes ?? new Dictionary<string,string>())}";
    }
}
