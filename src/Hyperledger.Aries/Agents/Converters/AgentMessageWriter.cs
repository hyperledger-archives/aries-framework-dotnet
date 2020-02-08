using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hyperledger.Aries.Agents
{
    internal class AgentMessageWriter : JsonConverter<AgentMessage>
    {
        public override void WriteJson(JsonWriter writer, AgentMessage value, JsonSerializer serializer)
        {
            var val = JObject.FromObject(value);
            var decorators = value.GetDecorators();

            foreach (var decorator in decorators)
                val.Add(decorator.Name, decorator.Value);

            serializer.Serialize(writer, val);
        }

        public override AgentMessage ReadJson(JsonReader reader, Type objectType, AgentMessage existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead => false;
        public override bool CanWrite => true;
    }
}