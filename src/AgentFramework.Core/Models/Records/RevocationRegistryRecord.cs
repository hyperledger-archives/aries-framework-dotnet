namespace AgentFramework.Core.Models.Records
{
    /// <summary>
    /// Represents revocation registry record.
    /// </summary>
    public class RevocationRegistryRecord : RecordBase
    {
        /// <summary>
        /// Gets or sets the revocation registry identifier.
        /// </summary>
        /// <value>The revocation registry identifier.</value>
        public string CredentialDefinitionId
        {
            get => Get();
            set => Set(value);
        }

        /// <summary>
        /// Gets or sets the tails file where the registry data is stored.
        /// </summary>
        /// <value>The tails file.</value>
        public string TailsFile
        {
            get => Get();
            set => Set(value);
        }

        /// <summary>
        /// Gets the name of the record type for this object.
        /// </summary>
        /// <returns>The type name.</returns>
        public override string TypeName => "AF.RevocationRegistryRecord";

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"CredentialDefinitionId={CredentialDefinitionId}, " +
            $"TailsFile={TailsFile}, " +
            base.ToString();
    }
}