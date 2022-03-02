using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.Handshakes.Common;
using Hyperledger.Aries.Features.Routing;
using Hyperledger.Indy.CryptoApi;
using Hyperledger.Indy.WalletApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hyperledger.Aries.Utils
{
    public class CryptoUtils
    {
        /// <summary>Packs a message</summary>
        /// <param name="wallet">The wallet.</param>
        /// <param name="recipientKey">The recipient key.</param>
        /// <param name="message">The message.</param>
        /// <param name="senderKey">The sender key.</param>
        /// <returns>Encrypted message formatted as JWE using UTF8 byte order</returns>
        public static Task<byte[]> PackAsync(
            Wallet wallet, string recipientKey, byte[] message, string senderKey = null) =>
            PackAsync(wallet, new[] { recipientKey }, message, senderKey);

        /// <summary>Packs the asynchronous.</summary>
        /// <param name="wallet">The wallet.</param>
        /// <param name="recipientKeys">The recipient keys.</param>
        /// <param name="message">The message.</param>
        /// <param name="senderKey">The sender key.</param>
        /// <returns>Encrypted message formatted as JWE using UTF8 byte order</returns>
        public static Task<byte[]> PackAsync(
            Wallet wallet, string[] recipientKeys, byte[] message, string senderKey = null) =>
            Crypto.PackMessageAsync(wallet, recipientKeys.ToJson(), senderKey, message);

        /// <summary>Packs the asynchronous.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="wallet">The wallet.</param>
        /// <param name="recipientKey">The recipient key.</param>
        /// <param name="message">The message.</param>
        /// <param name="senderKey">The sender key.</param>
        /// <returns>Encrypted message formatted as JWE using UTF8 byte order</returns>
        public static Task<byte[]> PackAsync<T>(
            Wallet wallet, string recipientKey, T message, string senderKey = null) =>
            PackAsync(wallet, new[] { recipientKey }, message.ToByteArray(), senderKey);

        /// <summary>Packs the asynchronous.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="wallet">The wallet.</param>
        /// <param name="recipientKeys">The recipient keys.</param>
        /// <param name="message">The message.</param>
        /// <param name="senderKey">The sender key.</param>
        /// <returns>Encrypted message formatted as JWE using UTF8 byte order</returns>
        public static Task<byte[]> PackAsync<T>(
            Wallet wallet, string[] recipientKeys, T message, string senderKey = null) =>
            Crypto.PackMessageAsync(wallet, recipientKeys.ToJson(), senderKey, message.ToByteArray());

        /// <summary>Unpacks the asynchronous.</summary>
        /// <param name="wallet">The wallet.</param>
        /// <param name="message">The message.</param>
        /// <returns>Decrypted message as UTF8 string and sender/recipient key information</returns>
        public static async Task<UnpackResult> UnpackAsync(Wallet wallet, byte[] message)
        {
            var result = await Crypto.UnpackMessageAsync(wallet, message);
            return result.ToObject<UnpackResult>();
        }

        /// <summary>Unpacks the asynchronous.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="wallet">The wallet.</param>
        /// <param name="message">The message.</param>
        /// <returns>Decrypted message as UTF8 string and sender/recipient key information</returns>
        public static async Task<T> UnpackAsync<T>(Wallet wallet, byte[] message)
        {
            var result = await Crypto.UnpackMessageAsync(wallet, message);
            var unpacked = result.ToObject<UnpackResult>();
            return unpacked.Message.ToObject<T>();
        }

        /// <summary>
        /// Generate unique random alpha-numeric key
        /// </summary>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        public static string GetUniqueKey(int maxSize)
        {
            var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            var data = new byte[maxSize];
            using (var crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
            }

            var result = new StringBuilder(maxSize);
            foreach (var b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }


        /// <summary>
        /// Prepares a wire level message from the application level agent message asynchronously 
        /// this includes packing the message and wrapping it in required forward messages
        /// if the message requires it.
        /// </summary>
        /// <param name="agentContext">The agentContext.</param>
        /// <param name="message">The message context.</param>
        /// <param name="recipientKey">The key to encrypt the message for.</param>
        /// <param name="routingKeys">The routing keys to pack the message for.</param>
        /// <param name="senderKey">The sender key to encrypt the message from.</param>
        /// <returns>The response async.</returns>
        public static async Task<byte[]> PrepareAsync(IAgentContext agentContext, AgentMessage message, string recipientKey, string[] routingKeys = null, string senderKey = null)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (recipientKey == null) throw new ArgumentNullException(nameof(recipientKey));
            
            recipientKey = DidUtils.IsDidKey(recipientKey) ? DidUtils.ConvertDidKeyToVerkey(recipientKey) : recipientKey;

            // Pack application level message
            var msg = await PackAsync(agentContext.Wallet, recipientKey, message.ToByteArray(), senderKey);

            var previousKey = recipientKey;

            if (routingKeys != null)
            {
                // TODO: In case of multiple key, should they each wrap a forward message
                // or pass all keys to the PackAsync function as array?
                foreach (var routingKey in routingKeys)
                {
                    var verkey = DidUtils.IsDidKey(routingKey) ? DidUtils.ConvertDidKeyToVerkey(routingKey) : routingKey;
                    // Anonpack
                    msg = await PackAsync(agentContext.Wallet, verkey, new ForwardMessage(agentContext.UseMessageTypesHttps) { Message = JObject.Parse(msg.GetUTF8String()), To = previousKey });
                    previousKey = verkey;
                }
            }

            return msg;
        }

        /// <summary>
        /// Prepares a wire level message from the application level agent message for a connection asynchronously 
        /// this includes packing the message and wrapping it in required forward messages
        /// if the message requires it.
        /// </summary>
        /// <param name="agentContext">The agentContext.</param>
        /// <param name="message">The message context.</param>
        /// <param name="connection">The connection to prepare the message for.</param>
        /// <returns>The response async.</returns>
        public static Task<byte[]> PrepareAsync(IAgentContext agentContext, AgentMessage message, ConnectionRecord connection)
        {
            var recipientKey = connection.TheirVk
                ?? throw new AriesFrameworkException(ErrorCode.A2AMessageTransmissionError, "Cannot find encryption key");

            var routingKeys = connection.Endpoint?.Verkey != null ? connection.Endpoint.Verkey : new string[0];
            return PrepareAsync(agentContext, message, recipientKey, routingKeys, connection.MyVk);
        }
    }

    /// <summary>
    /// Result object from <see cref="Crypto.UnpackMessageAsync"/>
    /// </summary>
    public class UnpackResult
    {
        /// <summary>Gets or sets the message encoded as UTF8 string.</summary>
        /// <value>The message.</value>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>Gets or sets the sender verkey.</summary>
        /// <value>The sender verkey.</value>
        [JsonProperty("sender_verkey")]
        [JsonPropertyName("sender_verkey")]
        public string SenderVerkey { get; set; }

        /// <summary>Gets or sets the recipient verkey.</summary>
        /// <value>The recipient verkey.</value>
        [JsonProperty("recipient_verkey")]
        [JsonPropertyName("recipient_verkey")]
        public string RecipientVerkey { get; set; }
    }
}
