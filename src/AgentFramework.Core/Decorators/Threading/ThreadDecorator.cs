using System.Collections.Generic;
using Newtonsoft.Json;

namespace AgentFramework.Core.Decorators.Threading
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
        public string ThreadId { get; set; }

        /// <summary>
        /// Parent thread id.
        /// </summary>
        [JsonProperty("pthid")]
        public string ParentThreadId { get; set; }

        /// <summary>
        /// Sender order.
        /// </summary>
        [JsonProperty("sender_order")]
        public int SenderOrder { get; set; }

        /// <summary>
        /// Received orders.
        /// </summary>
        [JsonProperty("received_orders")]
        public Dictionary<string, int> RecievedOrders { get; set; } = new Dictionary<string, int>();
    }
}
