using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AgentFramework.Core.Messages
{
    /// <summary>
    /// Represents an abstract base class of a content message.
    /// </summary>
    public abstract class AgentMessage
    {
        /// <summary>
        /// Internal JObject representation of an agent message.
        /// </summary>
        [JsonIgnore]
        private IList<JProperty> _decorators = new List<JProperty>();

        /// <summary>
        /// Gets or sets the message id.
        /// </summary>
        /// <value>
        /// The message id.
        /// </value>
        [JsonProperty("@id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [JsonProperty("@type")]
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
        /// <exception cref="AgentFrameworkException">ErrorCode.InvalidMessage Cannot deserialize packed message or unable to find decorator on message.</exception>
        public T GetDecorator<T>(string name) where T : class
        {
            try
            {
                var decorator = _decorators.First(_ => _.Name == $"~{name}");
                return decorator.Value.ToObject<T>();
            }
            catch (Exception e)
            {
                throw new AgentFrameworkException(ErrorCode.InvalidMessage, "Failed to extract decorator from message", e);
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