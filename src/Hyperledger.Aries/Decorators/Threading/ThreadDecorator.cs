using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Decorators.Threading
{
    /// <summary>
    /// Thread object decorator representation.
    /// </summary>
    public class ThreadDecorator
    {
        /// <summary>
        /// Thread id.
        /// </summary>
        [JsonProperty("thid")]
        [JsonPropertyName("thid")]
        public string ThreadId { get; set; }

        /// <summary>
        /// Parent thread id.
        /// </summary>
        [JsonProperty("pthid")]
        [JsonPropertyName("pthid")]
        public string ParentThreadId { get; set; }

        /// <summary>
        /// Sender order.
        /// </summary>
        [JsonProperty("sender_order")]
        [JsonPropertyName("sender_order")]
        public int SenderOrder { get; set; }

        /// <summary>
        /// Received orders.
        /// </summary>
        [JsonProperty("received_orders")]
        [JsonPropertyName("received_orders")]
        public Dictionary<string, int> ReceivedOrders { get; set; } = new Dictionary<string, int>();
    }
}
