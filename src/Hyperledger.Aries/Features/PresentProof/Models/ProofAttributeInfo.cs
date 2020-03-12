using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

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

        /// <summary>
        /// The attribute name and value to add as restriction.
        /// </summary>
        public AttributeValue AttributeValue { get; set; }
    }

    public class AttributeFilterConverter : JsonConverter<AttributeFilter>
    {
        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read. If there is no existing value then <c>null</c> will be used.</param>
        /// <param name="hasExistingValue">The existing value has a value.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The object value.
        /// </returns>
        public override AttributeFilter ReadJson(JsonReader reader, Type objectType, AttributeFilter existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var items = JObject.Load(reader);

            if (!hasExistingValue) existingValue = new AttributeFilter();

            serializer.Populate(items.CreateReader(), existingValue);

            var regex = new Regex("^attr::([^:]+)::(value)$");
            foreach (var item in items)
            {
                if (regex.IsMatch(item.Key))
                {
                    var match = regex.Match(item.Key);
                    existingValue.AttributeValue = new AttributeValue
                    {
                        Name = match.Groups[1].Value,
                        Value = item.Value.Value<string>()
                    };
                }
            }
            return existingValue;
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, AttributeFilter value, JsonSerializer serializer)
        {
            var dict = new Dictionary<string, string>();
            if (value.SchemaId != null) dict["schema_id"] = value.SchemaId;
            if (value.SchemaName != null) dict["schema_name"] = value.SchemaName;
            if (value.CredentialDefinitionId != null) dict["cred_def_id"] = value.CredentialDefinitionId;
            if (value.IssuerDid != null) dict["issuer_did"] = value.IssuerDid;
            if (value.SchemaIssuerDid != null) dict["schema_issuer_did"] = value.SchemaIssuerDid;
            if (value.SchemaVersion != null) dict["schema_version"] = value.SchemaVersion;
            if (value.AttributeValue != null) dict[$"attr::{value.AttributeValue.Name}::value"] = value.AttributeValue.Value;

            serializer.Serialize(writer, dict);
        }
    }

    public class AttributeValue
    {
        public string Name { get; set; }

        public string Value { get; set; }
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