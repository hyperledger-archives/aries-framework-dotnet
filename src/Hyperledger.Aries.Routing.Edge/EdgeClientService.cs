﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.DidExchange;
using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Routing
{
    public class EdgeClientService : IEdgeClientService
    {
        private const string MediatorInboxIdTagName = "MediatorInboxId";
        private const string MediatorInboxKeyTagName = "MediatorInboxKey";
        private const string MediatorConnectionIdTagName = "MediatorConnectionId";

        private readonly IProvisioningService provisioningService;
        private readonly IWalletRecordService recordService;
        private readonly IMessageService messageService;

        public EdgeClientService(
            IProvisioningService provisioningService,
            IWalletRecordService recordService,
            IMessageService messageService)
        {
            this.provisioningService = provisioningService;
            this.recordService = recordService;
            this.messageService = messageService;
        }

        public async Task AddRouteAsync(IAgentContext agentContext, string routeDestination)
        {
            var connection = await GetMediatorConnectionAsync(agentContext);
            if (connection != null)
            {
                var createInboxMessage = new AddRouteMessage { RouteDestination = routeDestination };
                await messageService.SendAsync(agentContext.Wallet, createInboxMessage, connection);
            }
        }

        public async Task CreateInboxAsync(IAgentContext agentContext, Dictionary<string, string> metadata = null)
        {
            var provisioning = await provisioningService.GetProvisioningAsync(agentContext.Wallet);
            if (provisioning.GetTag(MediatorInboxIdTagName) != null)
            {
                return;
            }
            var connection = await GetMediatorConnectionAsync(agentContext);

            var createInboxMessage = new CreateInboxMessage { Metadata = metadata };
            var response = await messageService.SendReceiveAsync<CreateInboxResponseMessage>(agentContext.Wallet, createInboxMessage, connection);

            provisioning.SetTag(MediatorInboxIdTagName, response.InboxId);
            provisioning.SetTag(MediatorInboxKeyTagName, response.InboxKey);
            await recordService.UpdateAsync(agentContext.Wallet, provisioning);
        }

        private async Task<ConnectionRecord> GetMediatorConnectionAsync(IAgentContext agentContext)
        {
            var provisioning = await provisioningService.GetProvisioningAsync(agentContext.Wallet);
            if (provisioning.GetTag(MediatorConnectionIdTagName) == null)
            {
                return null;
            }
            var connection = await recordService.GetAsync<ConnectionRecord>(agentContext.Wallet, provisioning.GetTag(MediatorConnectionIdTagName));
            if (connection == null) throw new AriesFrameworkException(ErrorCode.RecordNotFound, "Couldn't locate a connection to mediator agent");
            if (connection.State != ConnectionState.Connected) throw new AriesFrameworkException(ErrorCode.RecordInInvalidState, $"You must be connected to the mediator agent. Current state is {connection.State}");

            return connection;
        }

        public async Task<AgentPublicConfiguration> DiscoverConfigurationAsync(string agentEndpoint)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"{agentEndpoint}/.well-known/agent-configuration").ConfigureAwait(false);
            var responseJson = await response.Content.ReadAsStringAsync();

            return responseJson.ToObject<AgentPublicConfiguration>();
        }

        public async Task FetchInboxAsync(IAgentContext agentContext)
        {
            var connection = await GetMediatorConnectionAsync(agentContext);
            if (connection == null)
            {
                throw new InvalidOperationException("This agent is not configured with a mediator");
            }

            var createInboxMessage = new GetInboxItemsMessage();
            var response = await messageService.SendReceiveAsync<GetInboxItemsResponseMessage>(agentContext.Wallet, createInboxMessage, connection);

            var processedItems = new List<string>();
            foreach (var item in response.Items)
            {
                try
                {
                    await agentContext.Agent.ProcessAsync(agentContext, new PackedMessageContext(item.Data));
                    processedItems.Add(item.Id);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error processing message {e}");
                }
            }

            await messageService.SendAsync(agentContext.Wallet, new DeleteInboxItemsMessage { InboxItemIds = processedItems }, connection);
        }

        public async Task AddDeviceAsync(IAgentContext agentContext, AddDeviceInfoMessage message)
        {
            var connection = await GetMediatorConnectionAsync(agentContext);
            if (connection != null)
            {
                await messageService.SendAsync(agentContext.Wallet, message, connection);
            }
        }
    }
}