using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Features.Handshakes.Common;
using Hyperledger.Aries.Storage;
using Hyperledger.Indy.WalletApi;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hyperledger.Aries.Routing
{
    public class RoutingInboxHandler : IMessageHandler
    {
        private readonly IWalletRecordService recordService;
        private readonly IWalletService walletService;
        private readonly IRoutingStore routingStore;
        private readonly AgentOptions options;
        private readonly ILogger<RoutingInboxHandler> logger;
        private const string IndySdkDefaultOptions = "{}";

        public RoutingInboxHandler(
            IWalletRecordService recordService,
            IWalletService walletService,
            IRoutingStore routingStore,
            IOptions<AgentOptions> options,
            ILogger<RoutingInboxHandler> logger)
        {
            this.recordService = recordService;
            this.walletService = walletService;
            this.routingStore = routingStore;
            this.options = options.Value;
            this.logger = logger;
        }

        public IEnumerable<MessageType> SupportedMessageTypes => new MessageType[]
        {
            RoutingTypeNames.CreateInboxMessage,
            RoutingTypeNames.AddRouteMessage,
            RoutingTypeNames.AddDeviceInfoMessage,
            RoutingTypeNames.GetInboxItemsMessage,
            RoutingTypeNames.DeleteInboxItemsMessage
        };

        public async Task<AgentMessage> ProcessAsync(IAgentContext agentContext, UnpackedMessageContext messageContext)
        {
            if (messageContext.Connection == null ||
                messageContext.Connection.MultiPartyInvitation ||
                messageContext.Connection.State != ConnectionState.Connected)
            {
                throw new InvalidOperationException("Connection is missing or invalid");
            }

            switch (messageContext.GetMessageType())
            {
                case RoutingTypeNames.CreateInboxMessage:
                    return await CreateInboxAsync(agentContext, messageContext.Connection, messageContext.GetMessage<CreateInboxMessage>());

                case RoutingTypeNames.AddRouteMessage:
                    await AddRouteAsync(agentContext, messageContext.Connection, messageContext.GetMessage<AddRouteMessage>());
                    break;

                case RoutingTypeNames.GetInboxItemsMessage:
                    return await GetInboxItemsAsync(agentContext, messageContext.Connection, messageContext.GetMessage<GetInboxItemsMessage>());

                case RoutingTypeNames.DeleteInboxItemsMessage:
                    await DeleteInboxItemsAsync(agentContext, messageContext.Connection, messageContext.GetMessage<DeleteInboxItemsMessage>());
                    break;

                case RoutingTypeNames.AddDeviceInfoMessage:
                    await AddDeviceInfoAsync(agentContext, messageContext.Connection, messageContext.GetMessage<AddDeviceInfoMessage>());
                    break;

                default:
                    break;
            }

            return null;
        }

        private async Task AddDeviceInfoAsync(IAgentContext agentContext, ConnectionRecord connection, AddDeviceInfoMessage addDeviceInfoMessage)
        {
            var inboxId = connection.GetTag("InboxId");
            if (inboxId == null)
            {
                throw new InvalidOperationException("Inbox was not found. Create an inbox first");
            }

            var deviceRecord = new DeviceInfoRecord
            {
                InboxId = inboxId,
                DeviceId = addDeviceInfoMessage.DeviceId,
                DeviceVendor = addDeviceInfoMessage.DeviceVendor
            };
            try
            {
                await recordService.AddAsync(agentContext.Wallet, deviceRecord);
            }
            catch (WalletItemAlreadyExistsException)
            {
            }
            catch (Exception e)
            {
                logger.LogError(e, "Unable to register device", addDeviceInfoMessage);
            }
        }

        private async Task DeleteInboxItemsAsync(IAgentContext agentContext, ConnectionRecord connection, DeleteInboxItemsMessage deleteInboxItemsMessage)
        {
            var inboxId = connection.GetTag("InboxId");
            var inboxRecord = await recordService.GetAsync<InboxRecord>(agentContext.Wallet, inboxId);

            var edgeWallet = await walletService.GetWalletAsync(inboxRecord.WalletConfiguration, inboxRecord.WalletCredentials);

            foreach (var itemId in deleteInboxItemsMessage.InboxItemIds)
            {
                try
                {
                    await recordService.DeleteAsync<InboxItemRecord>(edgeWallet, itemId);
                }
                catch(Exception e)
                {
                    logger.LogError(e, "Couldn't delete inbox item {ItemId}", itemId);
                }
            }
        }

        private async Task<GetInboxItemsResponseMessage> GetInboxItemsAsync(IAgentContext agentContext, ConnectionRecord connection, GetInboxItemsMessage getInboxItemsMessage)
        {
            var inboxId = connection.GetTag("InboxId");
            var inboxRecord = await recordService.GetAsync<InboxRecord>(agentContext.Wallet, inboxId);

            var edgeWallet = await walletService.GetWalletAsync(inboxRecord.WalletConfiguration, inboxRecord.WalletCredentials);

            var items = await recordService.SearchAsync<InboxItemRecord>(edgeWallet);
            return new GetInboxItemsResponseMessage
            {
                Items = items
                    .OrderBy(x => x.Timestamp)
                    .Select(x => new InboxItemMessage { Id = x.Id, Data = x.ItemData, Timestamp = x.Timestamp })
                    .ToList()
            };
        }

        private async Task AddRouteAsync(IAgentContext _, ConnectionRecord connection, AddRouteMessage addRouteMessage)
        {
            var inboxId = connection.GetTag("InboxId");

            await routingStore.AddRouteAsync(addRouteMessage.RouteDestination, inboxId);
        }

        private async Task<CreateInboxResponseMessage> CreateInboxAsync(IAgentContext agentContext, ConnectionRecord connection, CreateInboxMessage createInboxMessage)
        {
            if (connection.State != ConnectionState.Connected)
            {
                throw new InvalidOperationException("Can't create inbox if connection is not in final state");
            }

            string inboxId = $"Edge{Guid.NewGuid().ToString("N")}";
            string inboxKey = await Wallet.GenerateWalletKeyAsync(IndySdkDefaultOptions);

            var inboxRecord = new InboxRecord
            {
                Id = inboxId,
                WalletConfiguration = new WalletConfiguration
                {
                    Id = inboxId,
                    StorageType = options.WalletConfiguration?.StorageType ?? "default",
                    StorageConfiguration = options.WalletConfiguration?.StorageConfiguration
                },
                WalletCredentials = new WalletCredentials
                {
                    Key = inboxKey,
                    KeyDerivationMethod = options.WalletCredentials?.KeyDerivationMethod,
                    StorageCredentials = options.WalletCredentials?.StorageCredentials
                }
            };
            connection.SetTag("InboxId", inboxId);

            await walletService.CreateWalletAsync(
                configuration: inboxRecord.WalletConfiguration,
                credentials: inboxRecord.WalletCredentials);

            if (createInboxMessage.Metadata != null)
            {
                foreach (var metadata in createInboxMessage.Metadata)
                {
                    inboxRecord.SetTag(metadata.Key, metadata.Value);
                }
            }

            await recordService.AddAsync(agentContext.Wallet, inboxRecord);
            await recordService.UpdateAsync(agentContext.Wallet, connection);

            return new CreateInboxResponseMessage
            {
                InboxId = inboxId,
                InboxKey = inboxKey
            };
        }
    }
}
