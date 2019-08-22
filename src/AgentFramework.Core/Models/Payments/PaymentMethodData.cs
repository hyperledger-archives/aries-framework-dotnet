using Newtonsoft.Json;

namespace AgentFramework.Core.Models.Payments
{
    /// <summary>
    /// Payment method data
    /// </summary>
    public class PaymentMethodData
    {
        /// <summary>
        /// Gets or sets the supported networks.
        /// </summary>
        /// <value>
        /// The supported networks.
        /// </value>
        [JsonProperty("supportedNetworks")]
        public string[] SupportedNetworks { get; set; }

        /// <summary>
        /// Gets or sets the payee identifier.
        /// </summary>
        /// <value>
        /// The payee identifier.
        /// </value>
        [JsonProperty("payeeId")]
        public string PayeeId { get; set; }
    }
}
