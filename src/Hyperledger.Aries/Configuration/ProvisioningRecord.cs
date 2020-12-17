using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Ledger;
using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Configuration
{
    /// <summary>
    /// Represents a provisioning record in the agency wallet
    /// </summary>
    /// <seealso cref="RecordBase" />
    public class ProvisioningRecord : RecordBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProvisioningRecord"/> class.
        /// </summary>
        public ProvisioningRecord()
        {
            Endpoint = new AgentEndpoint();
            Owner = new AgentOwner();
        }

        /// <summary>
        /// Record Identifier
        /// </summary>
        internal const string UniqueRecordId = "SingleRecord";

        /// <inheritdoc />
        public override string Id => UniqueRecordId;

        /// <inheritdoc />
        public override string TypeName => "AF.ProvisioningRecord";

        /// <summary>
        /// Gets or sets the endpoint information for the provisioned agent.
        /// </summary>
        /// <returns>The endpoint information for the provisioned agent</returns>
        public virtual AgentEndpoint Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the owner information for the provisioned agent.
        /// </summary>
        /// <returns>The owner information for the provisioned agent</returns>
        public virtual AgentOwner Owner { get; set; }

        /// <summary>
        /// Gets or sets the issuer did for the provisioned agent.
        /// </summary>
        /// <returns>The issuer did for the provisioned agent</returns>
        [Newtonsoft.Json.JsonProperty("_issuerDid")]
        public virtual string IssuerDid { get; set; }

        /// <summary>
        /// Gets or sets the issuer verkey for the provisioned agent.
        /// </summary>
        /// <returns>The issuer verkey for the provisioned agent</returns>
        [Newtonsoft.Json.JsonProperty("_issuerVerkey")]
        public virtual string IssuerVerkey { get; set; }


        /// <summary>
        /// Gets or sets the master key identifier for the provisioned agent.
        /// </summary>
        /// <returns>The master key identifier for the provisioned agent</returns>
        [Newtonsoft.Json.JsonProperty("_masterSecretId")]
        public virtual string MasterSecretId { get; set; }


        /// <summary>
        /// Gets or sets the tails base uri for the provisioned agent.
        /// </summary>
        /// <returns>The tails base uri for the provisioned agent</returns>
        [Newtonsoft.Json.JsonProperty("_tailsBaseUri")]
        public virtual string TailsBaseUri { get; set; }

        /// <summary>
        /// Gets or sets the value for UseMessageTypesHttps.
        /// Only affects messages created by the default services,
        /// if you create additional messages you have to set the useMessageTypesHttps via ctor too
        /// </summary>
        /// <value>True if to use UseMessageTypesHttps.</value>
        [Newtonsoft.Json.JsonProperty("_useMessageTypesHttps")]
        public virtual bool UseMessageTypesHttps { get; set; }

        /// <summary>
        /// Gets or sets the default payment address
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public string DefaultPaymentAddressId
        {
            get => Get();
            set => Set(value);
        }

        /// <summary>
        /// Gets or sets the issuer seed for deterministic 
        /// DID generation
        /// </summary>
        /// <value></value>
        public string IssuerSeed { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IndyTaaAcceptance" /> for this agent
        /// </summary>
        /// <value></value>
        public IndyTaaAcceptance TaaAcceptance { get; set; }
    }
}
