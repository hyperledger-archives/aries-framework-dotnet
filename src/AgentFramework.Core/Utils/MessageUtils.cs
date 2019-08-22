using System;
using System.Text.RegularExpressions;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Messages;
using Newtonsoft.Json;

namespace AgentFramework.Core.Utils
{
    /// <summary>
    /// Utilities for handling agent messages.
    /// </summary>
    public static class MessageUtils
    {
        private const string MessageTypeRegex = @"^(did:[a-z]+:[a-zA-z\d]+;spec)\/([a-z\S]+)\/([0-9].[0-9])\/([a-z\S]+)";

        /// <summary>
        /// The valid query parameters.
        /// </summary>
        public static string[] ValidQueryParameters = { "m", "c_i" };

        /// <summary>
        /// Encodes a message to a valid URL based format.
        /// </summary>
        /// <typeparam name="T">Type of the agent message.</typeparam>
        /// <param name="baseUrl">Base URL for encoding the message with.</param>
        /// <param name="message">Message to encode.</param>
        /// <returns>Encoded message as a valid URL.</returns>
        public static string EncodeMessageToUrlFormat<T>(string baseUrl, T message) where T : AgentMessage
        {
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));

            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (!Uri.IsWellFormedUriString(baseUrl, UriKind.Absolute))
                throw new ArgumentException("Not a valid URI", (nameof(baseUrl)));

            return EncodeMessageToUrlFormat(new Uri(baseUrl), message);
        }

        /// <summary>
        /// Encodes a message to a valid URL based format.
        /// </summary>
        /// <typeparam name="T">Type of the agent message.</typeparam>
        /// <param name="baseUrl">Base URL for encoding the message with.</param>
        /// <param name="message">Message to encode.</param>
        /// <returns>Encoded message as a valid URL.</returns>
        public static string EncodeMessageToUrlFormat<T>(Uri baseUrl, T message) where T : AgentMessage
        {
            if (baseUrl == null)
                throw new ArgumentNullException(nameof(baseUrl));

            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return $"{baseUrl}?m={message.ToJson().ToBase64()}";
        }

        /// <summary>
        /// Decodes a message from a valid URL based format.
        /// </summary>
        /// <param name="encodedMessage">Encoded message.</param>
        /// <returns>The agent message as a JSON string.</returns>
        public static string DecodeMessageFromUrlFormat(string encodedMessage)
        {
            if (string.IsNullOrEmpty(encodedMessage))
                throw new ArgumentNullException(nameof(encodedMessage));

            if (!Uri.IsWellFormedUriString(encodedMessage, UriKind.Absolute))
                throw new ArgumentException("Not a valid URI", (nameof(encodedMessage)));

            var uri = new Uri(encodedMessage);
            
            string messageBase64 = null;

            foreach(var queryParam in ValidQueryParameters)
            {
                try
                {
                    messageBase64 = uri.DecodeQueryParameters()[queryParam];
                    break;
                }
                catch (Exception) { }
            }

            if (messageBase64 == null)
            {
                throw new ArgumentException("Unable to find expected query parameter", (nameof(encodedMessage)));
            }

            return messageBase64.FromBase64();
        }

        /// <summary>
        /// Decodes a message from a valid URL based format.
        /// </summary>
        /// <param name="encodedMessage">Encoded message.</param>
        /// <returns>The agent message as a object.</returns>
        public static T DecodeMessageFromUrlFormat<T>(string encodedMessage)
        {
            var json = DecodeMessageFromUrlFormat(encodedMessage);

            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Decodes a message type into its composing elements.
        /// </summary>
        /// <param name="messageType">A message type in string representation.</param>
        /// <returns>A tuple of the elements composing a message type string representation.</returns>
        public static (string uri, string messageFamilyName, string messageVersion, string messageName) DecodeMessageTypeUri(string messageType)
        {
            if (string.IsNullOrEmpty(messageType))
                throw new ArgumentNullException(nameof(messageType));

            var regExMatches = Regex.Matches(messageType, MessageTypeRegex);

            if (regExMatches.Count != 1 || regExMatches[0].Groups.Count != 5)
                throw new AgentFrameworkException(ErrorCode.InvalidParameterFormat, $"{messageType} is an invalid message type");

            return (regExMatches[0].Groups[1].Value, regExMatches[0].Groups[2].Value, regExMatches[0].Groups[3].Value, regExMatches[0].Groups[4].Value);
        }
    }
}
