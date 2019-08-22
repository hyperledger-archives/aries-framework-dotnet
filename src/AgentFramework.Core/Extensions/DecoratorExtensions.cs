using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace AgentFramework.Core.Extensions
{
    /// <summary>
    /// Decorator Extensions.
    /// </summary>
    public static class DecoratorExtensions
    {
        /// <summary>
        /// Finds the decorator with the specified name or returns <code>null</code>.
        /// </summary>
        /// <typeparam name="T">Type to cast the decorator to.</typeparam>
        /// <param name="messageContext">Message Context.</param>
        /// <param name="name">The name.</param>
        /// <returns>The requested decorator or null</returns>
        public static T FindDecorator<T>(this MessageContext messageContext, string name) where T : class
        {
            if (messageContext.Packed)
                throw new AgentFrameworkException(ErrorCode.InvalidMessage, "Cannot fetch decorator from packed message.");

            var jObject = JsonConvert.DeserializeObject<JObject>( messageContext.GetMessageJson());

            var decorator = jObject.Properties().FirstOrDefault(_ => _.Name == $"~{name}");
            return decorator?.Value.ToObject<T>();
        }
    }
}
