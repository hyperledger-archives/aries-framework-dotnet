using System.Collections.Generic;
using System.Linq;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Models.Credentials;
using Hyperledger.Indy.AnonCredsApi;
using Newtonsoft.Json.Linq;

namespace AgentFramework.Core.Utils
{
    /// <summary>
    /// Credential utilities
    /// </summary>
    public class CredentialUtils
    {
        /// <summary>
        /// Formats the credential values into a JSON usable with the <see cref="AnonCreds"/> API
        /// </summary>
        /// <returns>The credential values.</returns>
        /// <param name="credentialAttributes">The credential attributes.</param>
        public static string FormatCredentialValues(IEnumerable<CredentialPreviewAttribute> credentialAttributes)
        {
            if (credentialAttributes == null)
                return null;

            var result = new Dictionary<string, Dictionary<string, string>>();
            foreach (var item in credentialAttributes)
            {
                switch (item.MimeType)
                {
                    case CredentialMimeTypes.TextMimeType:
                        result.Add(item.Name, FormatStringCredentialAttribute(item));
                        break;
                    default:
                        throw new AgentFrameworkException(ErrorCode.InvalidParameterFormat, $"{item.Name} mime type of {item.MimeType} not supported");
                }
            }
            return result.ToJson();
        }

        private static Dictionary<string, string> FormatStringCredentialAttribute(CredentialPreviewAttribute attribute)
        {
            return new Dictionary<string, string>()
            {
                {"raw", (string) attribute.Value},
                {"encoded", "1234567890"} //TODO Add value encoding
            };
        }

        /// <summary>
        /// Validates if the credential preview attribute is valid.
        /// </summary>
        /// <param name="attribute">Credential preview attribute.</param>
        public static void ValidateCredentialPreviewAttribute(CredentialPreviewAttribute attribute)
        {
            switch (attribute.MimeType)
            {
                case CredentialMimeTypes.TextMimeType:
                    break;
                default:
                    throw new AgentFrameworkException(ErrorCode.InvalidParameterFormat, $"{attribute.Name} mime type of {attribute.MimeType} not supported");
            }
        }

        /// <summary>
        /// Validates if the credential preview attributes are valid.
        /// </summary>
        /// <param name="attributes">Credential preview attributes.</param>
        public static void ValidateCredentialPreviewAttributes(IEnumerable<CredentialPreviewAttribute> attributes)
        {
            var validationErrors = new List<string>();

            foreach (var attribute in attributes)
            {
                try
                {
                    ValidateCredentialPreviewAttribute(attribute);
                }
                catch (AgentFrameworkException e)
                {
                    validationErrors.Add(e.Message);
                }
            }

            if (validationErrors.Any())
                throw new AgentFrameworkException(ErrorCode.InvalidParameterFormat, validationErrors.ToArray());
        }

        /// <summary>
        /// Casts an attribute value object to its respective type.
        /// </summary>
        /// <param name="attributeValue">Attribute value object.</param>
        /// <param name="mimeType">Mime type to cast the attribute value to.</param>
        /// <returns></returns>
        public static object CastAttribute(object attributeValue, string mimeType)
        {
            switch (mimeType)
            {
                case CredentialMimeTypes.TextMimeType:
                    return (string) attributeValue;
                default:
                    throw new AgentFrameworkException(ErrorCode.InvalidParameterFormat, $"Mime type of {mimeType} not supported");
            }
        }

        /// <summary>
        /// Casts an attribute value object to its respective type.
        /// </summary>
        /// <param name="attributeValue">Attribute value object.</param>
        /// <param name="mimeType">Mime type to cast the attribute value to.</param>
        /// <returns></returns>
        public static object CastAttribute(JToken attributeValue, string mimeType)
        {
            switch (mimeType)
            {
                case CredentialMimeTypes.TextMimeType:
                    return attributeValue.Value<string>();
                default:
                    throw new AgentFrameworkException(ErrorCode.InvalidParameterFormat, $"Mime type of {mimeType} not supported");
            }
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <param name="jsonAttributeValues">The json attribute values.</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetAttributes(string jsonAttributeValues)
        {
            if (string.IsNullOrEmpty(jsonAttributeValues))
                return new Dictionary<string, string>();

            var attributes = JObject.Parse(jsonAttributeValues);

            var result = new Dictionary<string, string>();
            foreach (var attribute in attributes)
                result.Add(attribute.Key, attribute.Value["raw"].ToString());                

            return result;
        }
    }
}
