using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hyperledger.Aries.Agents
{
    internal class AgentMessageReader<T> : JsonConverter where T : AgentMessage, new()
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override bool CanWrite => false;

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var item = JObject.Load(reader);

            var decorators = item.Properties().Where(_ => _.Name.StartsWith("~", StringComparison.Ordinal));

            var obj = new T();
            obj.SetDecorators(decorators.ToList());

            serializer.Populate(item.CreateReader(), obj);
            return obj;
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableFrom(typeof(T));
        }
    }
}