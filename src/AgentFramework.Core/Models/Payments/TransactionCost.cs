using AgentFramework.Core.Models.Records;

namespace AgentFramework.Core.Models.Payments
{
    /// <summary>
    /// Transaction cost data
    /// </summary>
    public class TransactionCost
    {
        /// <summary>
        /// Gets or sets the payment address used for paying for this cost.
        /// </summary>
        /// <value>
        /// The payment address.
        /// </value>
        public PaymentAddressRecord PaymentAddress { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        /// <value>
        /// The amount.
        /// </value>
        public ulong Amount { get; set; }

        /// <summary>
        /// Gets or sets the payment method.
        /// </summary>
        /// <value>
        /// The payment method.
        /// </value>
        public string PaymentMethod { get; set; }
    }
}
