using Newtonsoft.Json;

namespace AgentFramework.Core.Models.Payments
{
    /// <summary>
    /// Represents a payment source
    /// </summary>
    public class IndyPaymentInputSource
    {
        /// <summary>
        /// Gets or sets the payment address.
        /// </summary>
        /// <value>
        /// The payment address.
        /// </value>
        [JsonProperty("paymentAddress")]
        public string PaymentAddress { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        [JsonProperty("source")]
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        /// <value>
        /// The amount.
        /// </value>
        [JsonProperty("amount")]
        public ulong Amount { get; set; }

        /// <summary>
        /// Gets or sets the extra.
        /// </summary>
        /// <value>
        /// The extra.
        /// </value>
        [JsonProperty("extra")]
        public string Extra { get; set; }
    }
}
