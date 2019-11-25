using System;
using System.Threading;
using System.Threading.Tasks;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.DidExchange;
using Hyperledger.Aries.Storage;
using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.WalletApi;
using Microsoft.Extensions.Hosting;

namespace Hyperledger.Aries.Agents.Edge
{
    internal class MediatorProvisioningService : IHostedService
    {
        internal const string EdgeInvitationTagName = "EdgeInvitationId";
        internal const string InvitationTagName = "Invitation";

        private readonly IConnectionService connectionService;
        private readonly IProvisioningService provisioningService;
        private readonly IWalletRecordService recordService;
        private readonly IAgentProvider agentProvider;

        public MediatorProvisioningService(
            IConnectionService connectionService,
            IProvisioningService provisioningService,
            IWalletRecordService recordService,
            IAgentProvider agentProvider)
        {
            this.connectionService = connectionService;
            this.provisioningService = provisioningService;
            this.recordService = recordService;
            this.agentProvider = agentProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await provisioningService.ProvisionAgentAsync();
            }
            catch(WalletStorageException)
            {
                // OK
            }
            catch (WalletExistsException)
            {
                // OK
            }

            var agentContext = await agentProvider.GetContextAsync();
            var provsioningRecord = await provisioningService.GetProvisioningAsync(agentContext.Wallet);

            if (provsioningRecord.GetTag(EdgeInvitationTagName) != null)
            {
                return;
            }

            var (invitation, record) = await connectionService.CreateInvitationAsync(
                agentContext: agentContext,
                config: new InviteConfiguration { MultiPartyInvitation = true, AutoAcceptConnection = true });

            invitation.RoutingKeys = null;

            record.SetTag(InvitationTagName, invitation.ToJson());
            provsioningRecord.SetTag(EdgeInvitationTagName, record.Id);
            await recordService.UpdateAsync(agentContext.Wallet, provsioningRecord);
            await recordService.UpdateAsync(agentContext.Wallet, record);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
