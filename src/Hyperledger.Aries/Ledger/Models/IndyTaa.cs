using Newtonsoft.Json;

namespace Hyperledger.Aries.Ledger
{
    /// <summary>
    /// Indy Transaction Author Agreement Model
    /// </summary>
    public class IndyTaa
    {
        /// <summary>
        /// The text of the agreement
        /// </summary>
        /// <value></value>
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>
        /// The version of the agreement
        /// </summary>
        /// <value></value>
        [JsonProperty("version")]
        public string Version { get; set; }
    }
}