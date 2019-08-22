using AgentFramework.Core.Models.Records;
using Newtonsoft.Json.Linq;

namespace AgentFramework.Core.Models.EphemeralChallenge
{
    /// <summary>
    /// An ephemeral challenge contents representation.
    /// </summary>
    public class EphemeralChallengeContents
    {
        /// <summary>
        /// Type of the challenge.
        /// </summary>
        public ChallengeType Type { get; set; }

        /// <summary>
        /// Contents of the challenge.
        /// </summary>
        public JObject Contents { get; set; }
    }
}
