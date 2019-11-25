using Hyperledger.Aries.Configuration;

namespace Hyperledger.Aries.Routing
{
    public class EdgeAgentOptions : AgentOptions
    {
        /// <summary>
        /// Skips running the provisioning process, but still registers
        /// the required agent services.
        /// This allows more control over when the process is run and can be
        /// initiated manually by running <see cref="" />
        /// </summary>
        /// <value></value>
        public bool DelayProvisioning { get; set; }
    }
}
