using System;
using Newtonsoft.Json;

namespace AgentFramework.Core.Messages.Credentials
{
    /// <summary>
    /// A credential offer content message.
    /// </summary>
    public class CredentialOfferMessage : AgentMessage
    {
        /// <inheritdoc />
        public CredentialOfferMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypes.CredentialOffer;
        }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        /// <value>
        /// The comment.
        /// </value>
        [JsonProperty("comment", NullValueHandling = NullValueHandling.Ignore)]
        public string Comment { get; set; }
        
        /// <summary>
        /// Gets or sets the offer json.
        /// </summary>
        /// <value>
        /// The offer json.
        /// </value>
        [JsonProperty("offer_json")]
        public string OfferJson { get; set; }

        /// <summary>
        /// Gets or sets the credential preview.
        /// </summary>
        /// <value>
        /// The preview.
        /// </value>
        [JsonProperty("credential_preview")]
        public CredentialPreviewMessage Preview { get; set; }

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"Id={Id}, " +
            $"Type={Type}, " +
            $"OfferJson={(OfferJson?.Length > 0 ? "[hidden]" : null)}";
    }
}
