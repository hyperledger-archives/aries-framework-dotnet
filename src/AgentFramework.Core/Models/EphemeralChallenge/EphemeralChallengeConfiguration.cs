using AgentFramework.Core.Models.Records;

namespace AgentFramework.Core.Models.EphemeralChallenge
{
    /// <summary>
    /// Ephemeral challenge configuration.
    /// </summary>
    public class EphemeralChallengeConfiguration
    {
        /// <summary>
        /// Name of the challenge configuration.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type of challenge.
        /// </summary>
        public ChallengeType Type { get; set; }

        /// <summary>
        /// Contents of the challenge configuration.
        /// </summary>
        public dynamic Contents { get; set; }
    }
}
