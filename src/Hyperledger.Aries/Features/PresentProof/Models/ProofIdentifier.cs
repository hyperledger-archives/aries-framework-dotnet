using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.PresentProof
{
    /// <summary>
    /// Represents an individual proof identifier stored in a proof in the wallet.
    /// </summary>
    public class ProofIdentifier
    {
        /// <summary>
        /// Gets or sets the schema identifier.
        /// </summary>
        /// <value>
        /// The schema identifier.
        /// </value>
        [JsonProperty("schema_id")]
        public string SchemaId { get; set; }

        /// <summary>
        /// Gets or sets the credential definition identifier.
        /// </summary>
        /// <value>
        /// The credential definition identifier.
        /// </value>
        [JsonProperty("cred_def_id")]
        public string CredentialDefintionId { get; set; }

        /// <summary>
        /// Gets or sets the revocation registry identifier.
        /// </summary>
        /// <value>
        /// The revocation registry identifier.
        /// </value>
        [JsonProperty("rev_reg_id")]
        public string RevocationRegistryId { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"SchemaId={SchemaId}, " +
            $"CredentialDefintionId={CredentialDefintionId}, " +
            $"RevocationRegistryId={RevocationRegistryId}, " +
            $"Timestamp={Timestamp}";
    }
}
