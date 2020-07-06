using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Hyperledger.Aries.Agents;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.ProblemReport
{

    /// <inheritdoc />
    /// <summary>
    /// A general problem report message.
    /// </summary>
    public class ProblemReportMessage : AgentMessage
    {
        /// <summary>
        /// Comment.
        /// </summary>
        [JsonProperty("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Problem Items.
        /// </summary>
        [JsonProperty("problem_items")]
        [JsonPropertyName("problem_items")]
        public IList<Dictionary<string,string>> ProblemItems { get; set; }

        /// <summary>
        /// Retry party.
        /// </summary>
        [JsonProperty("who_retries")]
        [JsonPropertyName("credwho_retries_id")]
        public RetryParty RetryParty { get; set; }

        /// <summary>
        /// Problem impact severity.
        /// </summary>
        [JsonProperty("impact")]
        [JsonPropertyName("impact")]
        public ProblemImpact ProblemImpact { get; set; }

        /// <summary>
        /// Problem noticed at.
        /// </summary>
        [JsonProperty("noticed_time")]
        [JsonPropertyName("noticed_time")]
        public DateTime NoticedAt { get; set; }

        /// <summary>
        /// Tracking Uri.
        /// </summary>
        [JsonProperty("tracking-uri")]
        [JsonPropertyName("tracking-uri")]
        public string TrackingUri { get; set; }

        /// <summary>
        /// Tracking Uri.
        /// </summary>
        [JsonProperty("escalation-uri")]
        [JsonPropertyName("escalation-uri")]
        public string EscalationUri { get; set; }
    }
}
