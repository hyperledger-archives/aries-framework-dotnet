using Newtonsoft.Json;

namespace AgentFramework.Core.Models.Payments
{
    /// <summary>
    /// Payment amount
    /// </summary>
    public class PaymentAmount
    {
        /// <summary>
        /// Gets or sets the currency
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the payment value
        /// </summary>
        [JsonProperty("value")]
        public ulong Value { get; set; }

        /// <summary>
        /// Implicit assignment operator
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator PaymentAmount(ulong value)
        {
            return new PaymentAmount { Value = value };
        }
    }
}
