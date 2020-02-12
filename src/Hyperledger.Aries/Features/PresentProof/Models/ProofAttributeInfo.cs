using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace Hyperledger.Aries.Features.PresentProof
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
        /// Gets or sets the name of the attribute
        /// <remarks>
        /// You can only specify value for 'name' or 'names', but not both.
        /// </remarks>
        /// </summary>
        /// <value>The name.</value>
        [JsonProperty("name")]
        public string Name
        { 
            get => _name; 
            set
            {
                if (value != null)
                {
                    _names = null;
                }
                _name = value;
            }
        }
        private string _name;

        /// <summary>
        /// Gets or sets a collection of attribute names.
        /// <remarks>
        /// You can only specify value for 'name' or 'names', but not both.
        /// </remarks>
        /// </summary>
        /// <value>The name.</value>
        [JsonProperty("names")]
        public string[] Names
        { 
            get => _names; 
            set
            {
                if (value != null && value.Any())
                {
                    _name = null;
                }
                _names = value;
            }
        }
        private string[] _names;

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
    }
}