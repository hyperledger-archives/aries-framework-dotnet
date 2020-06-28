using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.IssueCredential
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
        [JsonPropertyName("attrs")]
        public Dictionary<string, string> Attributes { get; set; }

        /// <summary>
        /// Gets or sets the schema identifier associated with this credential.
        /// </summary>
        /// <value>
        /// The schema identifier.
        /// </value>
        [JsonProperty("schema_id")]
        [JsonPropertyName("schema_id")]
        public string SchemaId { get; set; }

        /// <summary>
        /// Gets or sets the credential definition identifier associated with this credential.
        /// </summary>
        /// <value>
        /// The credential definition identifier.
        /// </value>
        [JsonProperty("cred_def_id")]
        [JsonPropertyName("cred_def_id")]
        public string CredentialDefinitionId { get; set; }

        /// <summary>
        /// Gets or sets the revocation registry identifier if supported by the definition, otherwise <c>null</c>.
        /// </summary>
        /// <value>
        /// The revocation registry identifier.
        /// </value>
        [JsonProperty("rev_reg_id")]
        [JsonPropertyName("rev_reg_id")]
        public string RevocationRegistryId { get; set; }

        /// <summary>
        /// Gets or sets the credential revocation identifier if supported by the definition, otherwise <c>null</c>.
        /// </summary>
        /// <value>
        /// The credential revocation identifier.
        /// </value>
        [JsonProperty("cred_rev_id")]
        [JsonPropertyName("cred_rev_id")]
        public string CredentialRevocationId { get; set; }
    }
}
