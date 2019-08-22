using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Models.Records;

namespace AgentFramework.Core.Models.Wallets
{
    /// <summary>
    /// Basic provisioning configuration.
    /// </summary>
    public class BasicProvisioningConfiguration : ProvisioningConfiguration
    {
        /// <summary>
        /// Configures the agent wallet with basic configuration
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="record">Record.</param>
        /// <param name="context">Context.</param>
        public override Task ConfigureAsync(ProvisioningRecord record, IAgentContext context)
        {
            return Task.CompletedTask;
        }
    }
}
