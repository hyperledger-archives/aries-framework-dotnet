using AgentFramework.Core.Models.Payments;
using Newtonsoft.Json;

namespace AgentFramework.Core.Decorators.Payments
{
    /// <summary>
    /// Represents a payment receipt decorator
    /// <c>~payment_receipt</c>
    /// </summary>
    public class PaymentReceiptDecorator
    {
        /// <summary>
        /// Gets or set the request identifier
        /// </summary>
        /// <value>
        /// The request identifier.
        /// </value>
        [JsonProperty("request_id")]
        public string RequestId { get; set; }

        /// <summary>
        /// Gets or sets the selected method
        /// </summary>
        /// <value>
        /// The selected method.
        /// </value>
        [JsonProperty("selected_method")]
        public string SelectedMethod { get; set; }

        /// <summary>
        /// Gets or sets the transaction identifier.
        /// </summary>
        /// <value>
        /// The transaction identifier.
        /// </value>
        [JsonProperty("transaction_id")]
        public string TransactionId { get; set; }

        /// <summary>
        /// Gets or sets the payee identifier.
        /// </summary>
        /// <value>
        /// The payee identifier.
        /// </value>
        [JsonProperty("payeeId")]
        public string PayeeId { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        /// <value>
        /// The amount.
        /// </value>
        [JsonProperty("amount")]
        public PaymentAmount Amount { get; set; }
    }
}
