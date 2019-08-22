using System.Collections.Generic;
using Newtonsoft.Json;

namespace AgentFramework.Core.Models.Records.Search
{
    /// <summary>
    /// Search record result.
    /// </summary>
    public class SearchResult
    {
        /// <summary>
        /// Gets or sets the resulting records.
        /// </summary>
        /// <value>The resulting records.</value>
        [JsonProperty("records")]
        public List<SearchItem> Records { get; set; }

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"Records={string.Join(",", Records ?? new List<SearchItem>())}";
    }
}
