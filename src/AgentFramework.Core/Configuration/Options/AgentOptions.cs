namespace AgentFramework.Core.Configuration.Options
{
    /// <summary>
    /// Agent options
    /// </summary>
    public class AgentOptions
    {
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
    }
}