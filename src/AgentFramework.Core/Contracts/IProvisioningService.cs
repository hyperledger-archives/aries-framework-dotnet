using System;
using System.Threading.Tasks;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Models;
using AgentFramework.Core.Models.Records;
using AgentFramework.Core.Models.Wallets;
using Hyperledger.Indy.WalletApi;

namespace AgentFramework.Core.Contracts
{
    /// <summary>
    /// Provisioning Service.
    /// </summary>
    public interface IProvisioningService
    {
        /// <summary>
        /// Gets my endpoint asynchronous.
        /// </summary>
        /// <param name="wallet">The wallet.</param>
        /// <exception cref="AgentFrameworkException">Throws with ErrorCode.RecordNotFound.</exception>
        /// <returns>The provisioning record.</returns>
        Task<ProvisioningRecord> GetProvisioningAsync(Wallet wallet);

        /// <summary>
        /// Stores the endpoint asynchronous.
        /// </summary>
        /// <param name="wallet">The wallet.</param>
        /// <param name="provisioningConfiguration">The provisioning request.</param>
        /// <returns>The response async.</returns>
        [Obsolete]
        Task ProvisionAgentAsync(Wallet wallet, ProvisioningConfiguration provisioningConfiguration);

        /// <summary>
        /// Creates a wallet and provisions a new agent with the given <see cref="ProvisioningConfiguration" />
        /// </summary>
        /// <param name="configuration">The provisioning configuration.</param>
        /// <returns></returns>
        Task ProvisionAgentAsync(ProvisioningConfiguration configuration);

        /// <summary>
        /// Updates the agent endpoint information.
        /// </summary>
        /// <param name="wallet">The wallet.</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <returns></returns>
        Task UpdateEndpointAsync(Wallet wallet, AgentEndpoint endpoint);
    }
}
