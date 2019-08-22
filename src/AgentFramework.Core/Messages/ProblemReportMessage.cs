using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AgentFramework.Core.Messages
{
    /// <summary>
    /// Enumeration of possible retry parties.
    /// </summary>
    public enum RetryParty
    {
        /// <summary>
        /// You as the retry party.
        /// </summary>
        you,
        /// <summary>
        /// Me as the retry party.
        /// </summary>
        me,
        /// <summary>
        /// Both parties retry.
        /// </summary>
        both,
        /// <summary>
        /// No party retries.
        /// </summary>
        none
    }

    /// <summary>
    /// Problem impact severity.
    /// </summary>
    public enum ProblemImpact
    {
        /// <summary>
        /// Message level impact.
        /// </summary>
        message,
        /// <summary>
        /// Thread level impact.
        /// </summary>
        thread,
        /// <summary>
        /// Connection level impact.
        /// </summary>
        connection
    }

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
        public IList<Dictionary<string,string>> ProblemItems { get; set; }

        /// <summary>
        /// Retry party.
        /// </summary>
        [JsonProperty("who_retries")]
        public RetryParty RetryParty { get; set; }

        /// <summary>
        /// Problem impact severity.
        /// </summary>
        [JsonProperty("impact")]
        public ProblemImpact ProblemImpact { get; set; }

        /// <summary>
        /// Problem noticed at.
        /// </summary>
        [JsonProperty("noticed_time")]
        public DateTime NoticedAt { get; set; }

        /// <summary>
        /// Tracking Uri.
        /// </summary>
        [JsonProperty("tracking-uri")]
        public string TrackingUri { get; set; }

        /// <summary>
        /// Tracking Uri.
        /// </summary>
        [JsonProperty("escalation-uri")]
        public string EscalationUri { get; set; }
    }
}
