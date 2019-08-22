using System.Collections.Generic;
using Newtonsoft.Json;

namespace AgentFramework.Core.Models.Proofs
{
    /// <summary>Attribute filter</summary>
    public class AttributeFilter
    {
        /// <summary>The schema identifier</summary>
        [JsonProperty("schema_id", NullValueHandling = NullValueHandling.Ignore)]
        public string SchemaId { get; set; }

        /// <summary>The schema issuer did</summary>
        [JsonProperty("schema_issuer_did", NullValueHandling = NullValueHandling.Ignore)]
        public string SchemaIssuerDid { get; set; }

        /// <summary>The schema name</summary>
        [JsonProperty("schema_name", NullValueHandling = NullValueHandling.Ignore)]
        public string SchemaName { get; set; }

        /// <summary>The schema version</summary>
        [JsonProperty("schema_version", NullValueHandling = NullValueHandling.Ignore)]
        public string SchemaVersion { get; set; }

        /// <summary>The issuer did</summary>
        [JsonProperty("issuer_did", NullValueHandling = NullValueHandling.Ignore)]
        public string IssuerDid { get; set; }

        /// <summary>The credential definition identifier</summary>
        [JsonProperty("cred_def_id", NullValueHandling = NullValueHandling.Ignore)]
        public string CredentialDefinitionId { get; set; }
    }

    /// <summary>
    /// Proof attribute info.
    /// </summary>
    public class ProofAttributeInfo
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the restrictions.
        /// <code>
        /// filter_json: filter for credentials
        ///    {
        ///        "schema_id": string, (Optional)
        ///        "schema_issuer_did": string, (Optional)
        ///        "schema_name": string, (Optional)
        ///        "schema_version": string, (Optional)
        ///        "issuer_did": string, (Optional)
        ///        "cred_def_id": string, (Optional)
        ///    }
        /// </code>
        /// </summary>
        /// <value>The restrictions.</value>
        [JsonProperty("restrictions", NullValueHandling = NullValueHandling.Ignore)]
        public List<AttributeFilter> Restrictions { get; set; }

        /// <summary>
        /// Gets or sets the non revoked interval.
        /// </summary>
        /// <value>
        /// The non revoked.
        /// </value>
        [JsonProperty("non_revoked", NullValueHandling = NullValueHandling.Ignore)]
        public RevocationInterval NonRevoked { get; set; }
        
        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"Name={Name}, " +
            $"Restrictions={string.Join(",", Restrictions ?? new List<AttributeFilter>())}, " +
            $"NonRevoked={NonRevoked}";
    }
}