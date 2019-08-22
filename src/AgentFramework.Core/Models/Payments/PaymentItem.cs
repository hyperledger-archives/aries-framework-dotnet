using Newtonsoft.Json;
using System;
// ReSharper disable UseDeconstructionOnParameter

namespace AgentFramework.Core.Models.Payments
{
    /// <summary>
    /// Payment item
    /// </summary>
    public class PaymentItem
    {
        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        [JsonProperty("label")]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        /// <value>
        /// The amount.
        /// </value>
        [JsonProperty("amount")]
        public PaymentAmount Amount { get; set; }


        /// <summary>
        /// Performs an implicit conversion from <see cref="ValueTuple{String, UInt64}"/> to <see cref="PaymentItem"/>.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator PaymentItem((string label, ulong amount) item)
        {
            return new PaymentItem
            {
                Label = item.label,
                Amount = item.amount
            };
        }
    }
}
