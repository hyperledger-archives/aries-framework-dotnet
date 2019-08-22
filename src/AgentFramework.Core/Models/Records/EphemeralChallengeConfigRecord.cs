using Newtonsoft.Json.Linq;

namespace AgentFramework.Core.Models.Records
{
    /// <summary>
    /// Challenge type.
    /// </summary>
    public enum ChallengeType
    {
        /// <summary>
        /// The proof.
        /// </summary>
        Proof
    }

    /// <summary>
    /// Represents an ephemeral challenge configuration record in the agents wallet.
    /// </summary>
    /// <seealso cref="RecordBase" />
    public class EphemeralChallengeConfigRecord : RecordBase
    {
        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <returns>The type name.</returns>
        public override string TypeName => "AF.EphemeralChallengeConfigRecord";

        /// <summary>
        /// Gets the name of the challenge configuration.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the challenge configuration type.
        /// </summary>
        public ChallengeType Type { get; set; }

        /// <summary>
        /// Gets the challenge configuration contents.
        /// </summary>
        public JObject Contents { get; set; }

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"Contents={Contents}, " +
            $"Type={Type}, " +
            base.ToString();  
    }
}
