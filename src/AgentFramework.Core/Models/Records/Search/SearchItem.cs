using System.Collections.Generic;
using Newtonsoft.Json;

namespace AgentFramework.Core.Models.Records.Search
{
    /// <summary>
    /// Search record item.
    /// </summary>
    public class SearchItem
    {
        /// <summary>
        /// Gets or sets the records.
        /// </summary>
        /// <value>The records.</value>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the records.
        /// </summary>
        /// <value>The records.</value>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the records.
        /// </summary>
        /// <value>The records.</value>
        [JsonProperty("value")]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the records.
        /// </summary>
        /// <value>The records.</value>
        [JsonProperty("tags")]
        public Dictionary<string, string> Tags { get; set; }
        
        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"Id={Id}, " +
            $"Type={Type}, " +
            $"Value={Value}, " +
            $"Tags={string.Join(",", Tags ?? new Dictionary<string, string>())}";
    }
}
