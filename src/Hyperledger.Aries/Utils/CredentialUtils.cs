using System.Collections.Generic;
using System.Linq;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.IssueCredential;
using Newtonsoft.Json.Linq;
using Hyperledger.Indy.AnonCredsApi;
using System.Security.Cryptography;
using System;
using System.Numerics;

namespace Hyperledger.Aries.Utils
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
            var result = credentialAttributes?.ToDictionary(item => item.Name, FormatStringCredentialAttribute);
            return result?.ToJson();
        }

        static SHA256 sha256 = SHA256.Create();
        private static Dictionary<string, string> FormatStringCredentialAttribute(CredentialPreviewAttribute attribute)
        {
            return new Dictionary<string, string>()
            {
                {"raw", (string) attribute.Value},
                {"encoded", GetEncoded((string) attribute.Value)}
            };
        }

        internal static string GetEncoded(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) value = string.Empty;
            if (int.TryParse(value, out var result)) return result.ToString();

            var data = new byte[] { 0 }
                .Concat(sha256.ComputeHash(value.GetUTF8Bytes()))
                .ToArray();

            Array.Reverse(data);
            return new BigInteger(value: data).ToString();

            /*
                netstandard2.1 includes the ctor below,
                which allows to specify expected sign
                and endianess

            return new BigInteger(
                value: data,
                isUnsigned: true,
                isBigEndian: true).ToString();
            */
        }

        /// <summary>
        /// Checks if the value is encoded correctly
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public static bool CheckValidEncoding(string raw, string encoded)
        {
            if (string.IsNullOrWhiteSpace(raw)) raw = string.Empty;
            if (int.TryParse(raw, out var _)) return string.CompareOrdinal(raw, encoded) == 0;
            return string.CompareOrdinal(encoded, GetEncoded(raw)) == 0;
        }

        /// <summary>
        /// Validates if the credential preview attribute is valid.
        /// </summary>
        /// <param name="attribute">Credential preview attribute.</param>
        public static void ValidateCredentialPreviewAttribute(CredentialPreviewAttribute attribute)
        {
            switch (attribute.MimeType)
            {
                case null:
                case CredentialMimeTypes.TextMimeType:
                case CredentialMimeTypes.ApplicationJsonMimeType:
                case CredentialMimeTypes.ImagePngMimeType:
                    break;
                default:
                    throw new AriesFrameworkException(ErrorCode.InvalidParameterFormat, $"{attribute.Name} mime type of {attribute.MimeType} not supported");
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
                catch (AriesFrameworkException e)
                {
                    validationErrors.Add(e.Message);
                }
            }

            if (validationErrors.Any())
                throw new AriesFrameworkException(ErrorCode.InvalidParameterFormat, validationErrors.ToArray());
        }

        /// <summary>
        /// Casts an attribute value object to its respective type.
        /// </summary>
        /// <param name="attributeValue">Attribute value object.</param>
        /// <param name="mimeType">Mime type to cast the attribute value to.</param>
        /// <returns></returns>
        public static object CastAttribute(object attributeValue, string mimeType)
        {
            return (string)attributeValue;
        }

        /// <summary>
        /// Casts an attribute value object to its respective type.
        /// </summary>
        /// <param name="attributeValue">Attribute value object.</param>
        /// <param name="mimeType">Mime type to cast the attribute value to.</param>
        /// <returns></returns>
        public static object CastAttribute(JToken attributeValue, string mimeType)
        {
            return attributeValue.Value<string>();
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
