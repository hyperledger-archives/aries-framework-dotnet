﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Features.DidExchange;
using Hyperledger.Aries.Routing;
using Hyperledger.Aries.Storage;
using Hyperledger.Indy.WalletApi;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Hyperledger.Aries.Agents.Edge
{
    public class EdgeProvisioningService : IHostedService
    {
        private const string MediatorConnectionIdTagName = "MediatorConnectionId";
        private const string MediatorInboxIdTagName = "MediatorInboxId";

        private readonly IProvisioningService provisioningService;
        private readonly IConnectionService connectionService;
        private readonly IMessageService messageService;
        private readonly IEdgeClientService edgeClientService;
        private readonly IWalletRecordService recordService;
        private readonly IAgentProvider agentProvider;
        private readonly AgentOptions options;

        public EdgeProvisioningService(
            IProvisioningService provisioningService,
            IConnectionService connectionService,
            IMessageService messageService,
            IEdgeClientService edgeClientService,
            IWalletRecordService recordService,
            IAgentProvider agentProvider,
            IOptions<AgentOptions> options)
        {
            this.provisioningService = provisioningService;
            this.connectionService = connectionService;
            this.messageService = messageService;
            this.edgeClientService = edgeClientService;
            this.recordService = recordService;
            this.agentProvider = agentProvider;
            this.options = options.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var discovery = await edgeClientService.DiscoverConfigurationAsync(options.EndpointUri);
            
            try
            {
                options.AgentKey = discovery.RoutingKey;
                options.EndpointUri = discovery.ServiceEndpoint;

                await provisioningService.ProvisionAgentAsync(options);
            }
            catch (WalletExistsException)
            {
                // OK
            }
            var agentContext = await agentProvider.GetContextAsync();
            var provisioning = await provisioningService.GetProvisioningAsync(agentContext.Wallet);

            // Check if connection has been established with mediator agent
            if (provisioning.GetTag(MediatorConnectionIdTagName) == null)
            {
                var (request, record) = await connectionService.CreateRequestAsync(agentContext, discovery.Invitation);
                var response = await messageService.SendReceiveAsync<ConnectionResponseMessage>(agentContext.Wallet, request, record);
               
                await connectionService.ProcessResponseAsync(agentContext, response, record);
                
                // Remove the routing key explicitly as it won't ever be needed.
                // Messages will always be sent directly with return routing enabled
                record = await connectionService.GetAsync(agentContext, record.Id);
                record.Endpoint = new AgentEndpoint(record.Endpoint.Uri, null, null);
                await recordService.UpdateAsync(agentContext.Wallet, record);

                provisioning.SetTag(MediatorConnectionIdTagName, record.Id);
                await recordService.UpdateAsync(agentContext.Wallet, provisioning);
            }

            await edgeClientService.CreateInboxAsync(agentContext);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
