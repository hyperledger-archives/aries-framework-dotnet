using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace Hyperledger.Aries.Features.PresentProof
{
    /// <summary>
    /// Attribtue Filter Converter
    /// </summary>
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
}