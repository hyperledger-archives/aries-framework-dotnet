using System;
using System.Threading;
using System.Threading.Tasks;
using AgentFramework.Core.Configuration.Options;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Models;
using AgentFramework.Core.Models.Records;
using AgentFramework.Core.Models.Wallets;
using AgentFramework.Core.Utils;
using Hyperledger.Indy.AnonCredsApi;
using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.WalletApi;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace AgentFramework.Core.Handlers.Hosting
{
    /// <inheritdoc />
    public class DefaultProvisioningHostedService : IHostedService
    {
        private readonly IProvisioningService _provisioningService;
        
        /// <inheritdoc />
        public DefaultProvisioningHostedService(IProvisioningService provisioningService)
        {
            _provisioningService = provisioningService;
        }

        /// <inheritdoc />
        public async virtual Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _provisioningService.ProvisionAgentAsync();
            }
            catch(WalletExistsException)
            {
                // OK
            }
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}