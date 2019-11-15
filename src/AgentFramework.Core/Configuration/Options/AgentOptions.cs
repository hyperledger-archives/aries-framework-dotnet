using AgentFramework.Core.Models.Wallets;

namespace AgentFramework.Core.Configuration.Options
{
    /// <summary>
    /// Agent options
    /// </summary>
    public class AgentOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AgentOptions" /> class.
        /// </summary>
        public AgentOptions()
        {
            WalletConfiguration = new WalletConfiguration { Id = "DefaultWallet" };
            WalletCredentials = new WalletCredentials { Key = "DefaultKey" };
        }

        /// <summary>
        /// Gets or sets the wallet configuration.
        /// </summary>
        /// <value>The wallet configuration.</value>
        public WalletConfiguration WalletConfiguration
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the wallet credentials.
        /// </summary>
        /// <value>The wallet credentials.</value>
        public WalletCredentials WalletCredentials
        {
            get;
            set;
        }
        /// <summary>
        /// The DID of the issuer key pair
        /// </summary>
        /// <value></value>
        public string IssuerDid { get; set; }

        /// <summary>
        /// The key of the
        /// </summary>
        /// <value></value>
        public string IssuerKeySeed { get; set; }

        /// <summary>
        /// Gets or sets the agent did
        /// </summary>
        /// <value></value>
        public string AgentDid { get; set; }

        /// <summary>
        /// Gets or sets the agent key generation seed
        /// </summary>
        /// <value></value>
        public string AgentKeySeed { get; set; }

        /// <summary>
        /// Gets or sets the agent endpoint uri
        /// </summary>
        /// <value></value>
        public string EndpointUri { get; set; }

        /// <summary>
        /// Gets or sets the agent name used in connection invitations
        /// </summary>
        /// <value></value>
        public string AgentName { get; set; }

        /// <summary>
        /// Gets or sets the agent image uri
        /// </summary>
        /// <value></value>
        public string AgentImageUri { get; set; }

        /// <summary>
        /// The verification key of the agent
        /// </summary>
        /// <value></value>
        public string AgentKey { get; set; }

        /// <summary>
        /// Gets or sets the name of the pool.
        /// </summary>
        /// <value>The name of the pool.</value>
        public string PoolName
        {
            get;
            set;
        } = "DefaultPool";

        /// <summary>
        /// Gets or sets the genesis filename.
        /// </summary>
        /// <value>The genesis filename.</value>
        public string GenesisFilename
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the protocol version of the nodes.
        /// </summary>
        /// <value>
        /// The protocol version.
        /// </value>
        public int ProtocolVersion
        {
            get;
            set;
        } = 2;
    }
}