﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Decorators.Attachments;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.DidExchange;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Storage;
using Hyperledger.Indy.CryptoApi;
using Microsoft.Extensions.Options;
using Multiformats.Base;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Routing.Edge
{
    public partial class EdgeClientService : IEdgeClientService
    {
        private const string MediatorInboxIdTagName = "MediatorInboxId";
        private const string MediatorInboxKeyTagName = "MediatorInboxKey";
        private const string MediatorConnectionIdTagName = "MediatorConnectionId";
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IProvisioningService provisioningService;
        private readonly IWalletRecordService recordService;
        private readonly IWalletRecordService walletRecordService;
        private readonly IWalletService walletService;
        private readonly IMessageService messageService;

        private readonly AgentOptions agentoptions;

        public EdgeClientService(
            IHttpClientFactory httpClientFactory,
            IProvisioningService provisioningService,
            IWalletRecordService recordService,
            IMessageService messageService,
            IWalletRecordService walletRecordService,
            IWalletService walletService,
            IOptions<AgentOptions> agentOptions)
        {
            this.httpClientFactory = httpClientFactory;
            this.provisioningService = provisioningService;
            this.recordService = recordService;
            this.walletRecordService = walletRecordService;
            this.walletService = walletService;
            this.messageService = messageService;
            this.agentoptions = agentOptions.Value;
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

        internal async Task<ConnectionRecord> GetMediatorConnectionAsync(IAgentContext agentContext)
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
            var httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"{agentEndpoint}/.well-known/agent-configuration").ConfigureAwait(false);
            var responseJson = await response.Content.ReadAsStringAsync();

            return responseJson.ToObject<AgentPublicConfiguration>();
        }

        public async Task<(int, IEnumerable<InboxItemMessage>)> FetchInboxAsync(IAgentContext agentContext)
        {
            var connection = await GetMediatorConnectionAsync(agentContext);
            if (connection == null)
            {
                throw new InvalidOperationException("This agent is not configured with a mediator");
            }

            var createInboxMessage = new GetInboxItemsMessage();
            var response = await messageService.SendReceiveAsync<GetInboxItemsResponseMessage>(agentContext.Wallet, createInboxMessage, connection);

            var processedItems = new List<string>();
            var unprocessedItem = new List<InboxItemMessage>();
            foreach (var item in response.Items)
            {
                try
                {
                    await agentContext.Agent.ProcessAsync(agentContext, new PackedMessageContext(item.Data));
                    processedItems.Add(item.Id);
                }
                catch (Exception e)
                {
                    unprocessedItem.Add(item);
                }
            }

            if (processedItems.Any())
            {
                await messageService.SendAsync(agentContext.Wallet, new DeleteInboxItemsMessage { InboxItemIds = processedItems }, connection);
            }

            return (processedItems.Count, unprocessedItem);
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