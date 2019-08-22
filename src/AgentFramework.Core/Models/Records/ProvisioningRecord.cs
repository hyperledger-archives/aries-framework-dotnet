using Newtonsoft.Json;

namespace AgentFramework.Core.Models.Records
{
    /// <summary>
    /// Represents a provisioning record in the agency wallet
    /// </summary>
    /// <seealso cref="RecordBase" />
    public class ProvisioningRecord : RecordBase
    {
        [JsonProperty]
        private string _tailsBaseUri;
        [JsonProperty]
        private string _masterSecretId;
        [JsonProperty]
        private string _issuerVerkey;
        [JsonProperty]
        private string _issuerDid;

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
        public virtual AgentEndpoint Endpoint
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the owner information for the provisioned agent.
        /// </summary>
        /// <returns>The owner information for the provisioned agent</returns>
        public virtual AgentOwner Owner
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the issuer did for the provisioned agent.
        /// </summary>
        /// <returns>The issuer did for the provisioned agent</returns>
        [JsonIgnore]
        public virtual string IssuerDid
        {
            get => _issuerDid;
            internal set => _issuerDid = value;
        }

        /// <summary>
        /// Gets or sets the issuer verkey for the provisioned agent.
        /// </summary>
        /// <returns>The issuer verkey for the provisioned agent</returns>
        [JsonIgnore]
        public virtual string IssuerVerkey
        {
            get => _issuerVerkey;
            internal set => _issuerVerkey = value;
        }

        /// <summary>
        /// Gets or sets the master key identifier for the provisioned agent.
        /// </summary>
        /// <returns>The master key identifier for the provisioned agent</returns>
        [JsonIgnore]
        public virtual string MasterSecretId
        {
            get => _masterSecretId;
            internal set => _masterSecretId = value;
        }

        /// <summary>
        /// Gets or sets the tails base uri for the provisioned agent.
        /// </summary>
        /// <returns>The tails base uri for the provisioned agent</returns>
        [JsonIgnore]
        public virtual string TailsBaseUri
        {
            get => _tailsBaseUri;
            internal set => _tailsBaseUri = value;
        }

        /// <summary>
        /// Gets or sets the default payment address
        /// </summary>
        [JsonIgnore]
        public string DefaultPaymentAddressId
        {
            get => Get();
            set => Set(value);
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"Endpoint={Endpoint}, " +
            $"Controller={Owner}, " +
            $"IssuerDid={IssuerDid}, " +
            $"IssuerVerkey={(IssuerVerkey?.Length > 0 ? "[hidden]" : null)}, " +
            $"MasterSecretId={(MasterSecretId?.Length > 0 ? "[hidden]" : null)}, " +
            $"TailsBaseUri={TailsBaseUri}, " +
            base.ToString(); 
    }
}
