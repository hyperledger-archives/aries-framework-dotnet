using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hyperledger.Aries.Agents
{
    internal class AgentEndpointJsonConverter : JsonConverter<AgentEndpoint>
    {
        public override AgentEndpoint ReadJson(JsonReader reader, Type objectType, AgentEndpoint existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            existingValue ??= new AgentEndpoint();

            var items = JObject.Load(reader);
            if (items["verkey"] is JArray)
            {
                serializer.Populate(items.CreateReader(), existingValue);
                return existingValue;
            }

            existingValue.Did = items["did"]?.ToString();
            existingValue.Uri = items["uri"]?.ToString();
            existingValue.Verkey = items["verkey"] is null ? null : new[] { items["verkey"]?.ToString() };
            return existingValue;
        }

        public override bool CanWrite => false;
        public override void WriteJson(JsonWriter writer, AgentEndpoint value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}