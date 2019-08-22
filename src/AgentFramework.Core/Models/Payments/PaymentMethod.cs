using Newtonsoft.Json;

namespace AgentFramework.Core.Models.Payments
{
    /// <summary>
    /// Payment method
    /// </summary>
    public class PaymentMethod
    {
        /// <summary>
        /// Gets or sets the supported methods.
        /// </summary>
        /// <value>
        /// The supported methods.
        /// </value>
        [JsonProperty("supportedMethods")]
        public string SupportedMethods { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        [JsonProperty("data")]
        public PaymentMethodData Data { get; set; }
    }
}
