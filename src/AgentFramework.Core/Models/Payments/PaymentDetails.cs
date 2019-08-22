using System.Collections.Generic;
using Newtonsoft.Json;

namespace AgentFramework.Core.Models.Payments
{
    /// <summary>
    /// Payment details
    /// </summary>
    public class PaymentDetails
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        [JsonProperty("displayItems")]
        public IList<PaymentItem> Items { get; set; }

        /// <summary>
        /// Gets or sets the total.
        /// </summary>
        /// <value>
        /// The total.
        /// </value>
        [JsonProperty("total")]
        public PaymentItem Total { get; set; }
    }
}
