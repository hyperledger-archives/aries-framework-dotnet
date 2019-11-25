using Hyperledger.Aries;
using Hyperledger.Aries.Agents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace System
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
        public static T FindDecorator<T>(this UnpackedMessageContext messageContext, string name) where T : class
        {
            if (messageContext.Packed)
                throw new AriesFrameworkException(ErrorCode.InvalidMessage, "Cannot fetch decorator from packed message.");

            var jObject = JsonConvert.DeserializeObject<JObject>(messageContext.GetMessageJson());

            var decorator = jObject.Properties().FirstOrDefault(_ => _.Name == $"~{name}");
            return decorator?.Value.ToObject<T>();
        }
    }
}
