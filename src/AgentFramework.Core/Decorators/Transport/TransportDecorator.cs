using Newtonsoft.Json;

namespace AgentFramework.Core.Decorators.Transport
{
    /// <summary>
    /// Return route types.
    /// </summary>
    public enum ReturnRouteTypes
    {
        /// <summary>
        /// No messages should be returned over this connection.
        /// </summary>
        none,
        /// <summary>
        /// All messages for this key should be returned over this connection.
        /// </summary>
        all,
        /// <summary>
        /// Send all messages matching this thread over this conneciton.
        /// </summary>
        thread
    }

    /// <summary>
    /// Represents an attachment decorator <code>~transport</code>
    /// </summary>
    public class TransportDecorator
    {
        /// <summary>
        /// Return route.
        /// </summary>
        [JsonProperty("return_route")]
        public string ReturnRoute { get; set; }

        /// <summary>
        /// Return route thread.
        /// </summary>
        [JsonProperty("return_route_thread")]
        public string ReturnRouteThread { get; set; }

        /// <summary>
        /// Queued message count.
        /// </summary>
        [JsonProperty("queued_message_count")]
        public string QueuedMessageCount { get; set; }
    }
}
