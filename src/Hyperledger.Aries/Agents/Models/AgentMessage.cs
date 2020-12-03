using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;
using Hyperledger.Aries.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hyperledger.Aries.Agents
{
    /// <summary>
    /// Represents an abstract base class of a content message.
    /// </summary>
    public abstract class AgentMessage
    {
        public AgentMessage()
        {
            UseMessageTypesHttps = false;
        }

        public AgentMessage(bool useMessageTypesHttps)
        {
            UseMessageTypesHttps = useMessageTypesHttps;
        }

        /// <summary>
        /// Gets or sets if to use https messages.
        /// </summary>
        /// <value>
        /// If to use https messages.
        /// </value>
        protected bool UseMessageTypesHttps { get; set; }

        /// <summary>
        /// Internal JObject representation of an agent message.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        private IList<JProperty> _decorators = new List<JProperty>();

        /// <summary>
        /// Gets or sets the message id.
        /// </summary>
        /// <value>
        /// The message id.
        /// </value>
        [JsonProperty("@id")]
        [JsonPropertyName("@id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [JsonProperty("@type")]
        [JsonPropertyName("@type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets the decorators on the message.
        /// </summary>
        /// <returns>The decorators as a JObject.</returns>
        public IReadOnlyList<JProperty> GetDecorators() => new ReadOnlyCollection<JProperty>(_decorators);

        /// <summary>
        /// Internal set method for setting the collection of decorators attached to the message.
        /// </summary>
        /// <param name="decorators">JObject of decorators attached to the message.</param>
        internal void SetDecorators(IList<JProperty> decorators) => _decorators = decorators;

        /// <summary>
        /// Gets the decorator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="AriesFrameworkException">ErrorCode.InvalidMessage Cannot deserialize packed message or unable to find decorator on message.</exception>
        public T GetDecorator<T>(string name) where T : class
        {
            try
            {
                var decorator = _decorators.First(_ => _.Name == $"~{name}");
                return decorator.Value.ToObject<T>();
            }
            catch (Exception e)
            {
                throw new AriesFrameworkException(ErrorCode.InvalidMessage, "Failed to extract decorator from message", e);
            }
        }

        /// <summary>
        /// Finds the decorator with the specified name or returns <code>null</code>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns>The requested decorator or null</returns>
        public T FindDecorator<T>(string name) where T : class
        {
            var decorator = _decorators.FirstOrDefault(_ => _.Name == $"~{name}");
            return decorator?.Value.ToObject<T>();
        }

        /// <summary>
        /// Adds the decorator.
        /// </summary>
        /// <param name="decorator">The decorator.</param>
        /// <param name="name">The decorator name.</param>
        public void AddDecorator<T>(T decorator, string name) where T : class =>
            _decorators.Add(new JProperty($"~{name}", JsonConvert.DeserializeObject<JToken>(decorator.ToJson())));

        /// <summary>
        /// Sets the decorator overriding any existing values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="decorator">The decorator.</param>
        /// <param name="name">The name.</param>
        public void SetDecorator<T>(T decorator, string name) where T : class
        {
            _decorators.Remove(_decorators.FirstOrDefault(x => x.Name == $"~{name}"));
            AddDecorator(decorator, name);
        }
    }
}
