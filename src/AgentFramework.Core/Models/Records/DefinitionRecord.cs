using Newtonsoft.Json;

namespace AgentFramework.Core.Models.Records
{
    /// <summary>
    /// Schema definition record.
    /// </summary>
    public class DefinitionRecord : RecordBase
    {
        /// <summary>
        /// Gets or sets the identifier of the schema the definition is derived from.
        /// </summary>
        /// <value>The schema identifier.</value>
        [JsonIgnore]
        public string SchemaId
        {
            get => Get();
            set => Set(value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this definition supports credential revocation.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this definition supports credential revocation; otherwise, <c>false</c>.
        /// </value>
        public bool SupportsRevocation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether requests are automatically issued a credential.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [require approval]; otherwise, <c>false</c>.
        /// </value>
        public bool RequireApproval
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <returns>The type name.</returns>
        public override string TypeName => "AF.CredentialDefinition";

        /// <summary>Gets or sets the maximum credential count.</summary>
        /// <value>The maximum credential count.</value>
        public int MaxCredentialCount { get; set; }
        
        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"SchemaId={SchemaId}, " +
            $"SupportsRevocation={SupportsRevocation}, " +
            $"RequireApproval={RequireApproval}, " +
            $"MaxCredentialCount={MaxCredentialCount}, " +
            base.ToString(); 
    }
}