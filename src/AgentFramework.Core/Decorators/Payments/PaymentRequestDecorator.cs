using AgentFramework.Core.Models.Payments;
using Newtonsoft.Json;

namespace AgentFramework.Core.Decorators.Payments
{
    /// <summary>
    /// Payment request decorator
    /// </summary>
    public class PaymentRequestDecorator
    {
        /// <summary>
        /// The decorator label
        /// </summary>
        public const string Label = "payment_request";

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        [JsonProperty("methodData")]
        public PaymentMethod Method { get; set; }

        /// <summary>
        /// Gets or sets the details.
        /// </summary>
        /// <value>
        /// The details.
        /// </value>
        [JsonProperty("details")]
        public PaymentDetails Details { get; set; }
    }
}
