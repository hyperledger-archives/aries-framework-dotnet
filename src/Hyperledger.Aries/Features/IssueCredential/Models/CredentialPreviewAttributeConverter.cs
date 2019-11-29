using System;
using Hyperledger.Aries.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hyperledger.Aries.Features.IssueCredential
{
    internal class CredentialPreviewAttributeConverter : JsonConverter<CredentialPreviewAttribute>
    {
        public override void WriteJson(JsonWriter writer, CredentialPreviewAttribute value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override CredentialPreviewAttribute ReadJson(JsonReader reader, Type objectType, CredentialPreviewAttribute existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var item = JObject.Load(reader);

            var name = item["name"];
            var value = item["value"];
            var mimeType = item["mime-type"];

            var obj = new CredentialPreviewAttribute();
            obj.Name = name.Value<string>();
            obj.MimeType = mimeType?.Value<string>();
            obj.Value = CredentialUtils.CastAttribute(value, obj.MimeType);
            return obj;
        }

        public override bool CanRead => true;
        public override bool CanWrite => false;
    }
}