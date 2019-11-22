using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Ledger
{
    /// <summary>
    /// Acceptance Mechanisms List Data
    /// </summary>
    public class IndyAml
    {
        /// <summary>
        /// AML Context
        /// </summary>
        /// <value></value>
        [JsonProperty("amlContext")]
        public string AmlContext { get; set; }
        /// <summary>
        /// AML Version
        /// </summary>
        /// <value></value>
        [JsonProperty("version")]
        public string Version { get; set; }
        /// <summary>
        /// Acceptance Mechanisms List
        /// </summary>
        /// <value></value>
        [JsonProperty("aml")]
        public Dictionary<string, string> Aml { get; set; }
    }
}