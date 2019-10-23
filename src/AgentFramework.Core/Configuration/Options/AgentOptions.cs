namespace AgentFramework.Core.Configuration.Options
{
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

        public string AgentDid { get; set; }

        public string AgentKeySeed { get; set; }

        public string EndpointUri { get; set; }

        public string AgentName { get; set; }

        public string AgentImageUri { get; set; }

        /// <summary>
        /// The verification key of the agent
        /// </summary>
        /// <value></value>
        public string AgentKey { get; set; }
    }
}