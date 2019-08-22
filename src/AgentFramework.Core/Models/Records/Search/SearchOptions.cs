using Newtonsoft.Json;

namespace AgentFramework.Core.Models.Records.Search
{
    /// <summary>
    /// Search record options.
    /// </summary>
    public class SearchOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AgentFramework.Core.Models.Records.Search.SearchRecordOptions"/>
        /// retrieve records.
        /// </summary>
        /// <value><c>true</c> if retrieve records; otherwise, <c>false</c>.</value>
        [JsonProperty("retrieveRecords")]
        public bool RetrieveRecords { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AgentFramework.Core.Models.Records.Search.SearchRecordOptions"/>
        /// retrieve total count.
        /// </summary>
        /// <value><c>true</c> if retrieve total count; otherwise, <c>false</c>.</value>
        [JsonProperty("retrieveTotalCount")]
        public bool RetrieveTotalCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AgentFramework.Core.Models.Records.Search.SearchRecordOptions"/>
        /// retrieve type.
        /// </summary>
        /// <value><c>true</c> if retrieve type; otherwise, <c>false</c>.</value>
        [JsonProperty("retrieveType")]
        public bool RetrieveType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AgentFramework.Core.Models.Records.Search.SearchRecordOptions"/>
        /// retrieve value.
        /// </summary>
        /// <value><c>true</c> if retrieve value; otherwise, <c>false</c>.</value>
        [JsonProperty("retrieveValue")]
        public bool RetrieveValue { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AgentFramework.Core.Models.Records.Search.SearchRecordOptions"/>
        /// retrieve tags.
        /// </summary>
        /// <value><c>true</c> if retrieve tags; otherwise, <c>false</c>.</value>
        [JsonProperty("retrieveTags")]
        public bool RetrieveTags { get; set; } = true;
        
        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"RetrieveRecords={RetrieveRecords}, " +
            $"RetrieveTotalCount={RetrieveTotalCount}, " +
            $"RetrieveType={RetrieveType}, " +
            $"RetrieveValue={RetrieveValue}, " +
            $"RetrieveTags={RetrieveTags}";
    }
}
