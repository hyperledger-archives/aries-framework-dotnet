using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Features.DidExchange;
using Hyperledger.Aries.Routing;
using Hyperledger.Aries.Storage;
using Hyperledger.Indy.WalletApi;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Hyperledger.Aries.Tests.Routing
{
    public class RoutingInboxHandlerTests
    {
        private readonly Mock<IWalletRecordService> recordService;
        private readonly Mock<IWalletService> walletService;
        private readonly Mock<IRoutingStore> routingStore;
        private readonly Mock<IAgentContext> agentContext;
        private readonly Mock<ILogger<RoutingInboxHandler>> logger;
        private RoutingInboxHandler routingInboxHandler;

        public RoutingInboxHandlerTests()
        {
            recordService = new Mock<IWalletRecordService>();
            walletService = new Mock<IWalletService>();
            routingStore = new Mock<IRoutingStore>();
            agentContext = new Mock<IAgentContext>();
            IOptions<AgentOptions> ooptions = Options.Create<AgentOptions>(new AgentOptions());
            logger = new Mock<ILogger<RoutingInboxHandler>>();
            routingInboxHandler = new RoutingInboxHandler(recordService.Object, walletService.Object, routingStore.Object, ooptions, logger.Object);
        }
        [Fact(DisplayName = "Create inbox method should create inbox record and wallet record")]
        public async Task CreateInboxRecordAsync()
        {

            UnpackedMessageContext unpackedMessage = new UnpackedMessageContext(
                new CreateInboxMessage(),
                new ConnectionRecord()
                {
                    State = ConnectionState.Connected,
                });

            CreateInboxResponseMessage agentMessage = (CreateInboxResponseMessage)await routingInboxHandler.ProcessAsync(agentContext.Object, unpackedMessage);

            walletService.Verify(w => w.CreateWalletAsync(It.Is<WalletConfiguration>(wc => wc.Id == agentMessage.InboxId), It.Is<WalletCredentials>(wc => wc.Key == agentMessage.InboxKey)));
            recordService.Verify(m => m.AddAsync(agentContext.Object.Wallet, It.Is<InboxRecord>(i => i.Tags.Count == 0)), Times.Once());
            recordService.Verify(m => m.UpdateAsync(agentContext.Object.Wallet, It.Is<ConnectionRecord>(c => c.GetTag("InboxId") == agentMessage.InboxId)));
        }

        [Fact(DisplayName = "Create inbox method should create inbox record and wallet record and append items from Metadata to inboxRecord tags")]
        public async Task CreateInboxRecordWithMetadataAsync()
        {
            string key = "key", value = "value";
            Dictionary<string, string> metadata = new Dictionary<string, string>()
            {
                { key, value }
            };
            UnpackedMessageContext unpackedMessage = new UnpackedMessageContext(
                new CreateInboxMessage()
                {
                    Metadata = metadata
                },
                new ConnectionRecord()
                {
                    State = ConnectionState.Connected,
                });

            CreateInboxResponseMessage agentMessage = (CreateInboxResponseMessage)await routingInboxHandler.ProcessAsync(agentContext.Object, unpackedMessage);

            walletService.Verify(w => w.CreateWalletAsync(It.Is<WalletConfiguration>(wc => wc.Id == agentMessage.InboxId), It.Is<WalletCredentials>(wc => wc.Key == agentMessage.InboxKey)));
            recordService.Verify(m => m.AddAsync(agentContext.Object.Wallet, It.Is<InboxRecord>(i => i.GetTag(key) == value)), Times.Once());
            recordService.Verify(m => m.UpdateAsync(agentContext.Object.Wallet, It.Is<ConnectionRecord>(c => c.GetTag("InboxId") == agentMessage.InboxId)));
        }

        [Fact(DisplayName = "Create inbox method should create wallet record with storage configuration and credentials")]
        public async Task CreateInboxRecordWithStorageConfigurationAndCredentialsAsync()
        {
            object storageCredentials = new {};
            WalletConfiguration.WalletStorageConfiguration storageConfiguration = new WalletConfiguration.WalletStorageConfiguration();
            string storageType = "postgres_storage";
            IOptions<AgentOptions> options = Options.Create<AgentOptions>(new AgentOptions()
            {
                WalletConfiguration = new WalletConfiguration()
                {
                    StorageType = storageType,
                    StorageConfiguration = storageConfiguration
                },
                WalletCredentials = new WalletCredentials()
                {
                    StorageCredentials = storageCredentials
                }
            } );
            UnpackedMessageContext unpackedMessage = new UnpackedMessageContext(
                new CreateInboxMessage(),
                new ConnectionRecord()
                {
                    State = ConnectionState.Connected,
                });
            routingInboxHandler = new RoutingInboxHandler(recordService.Object, walletService.Object, routingStore.Object, options, logger.Object);

            CreateInboxResponseMessage agentMessage = (CreateInboxResponseMessage) await routingInboxHandler.ProcessAsync(agentContext.Object, unpackedMessage);

            walletService.Verify(w => w.CreateWalletAsync(It.Is<WalletConfiguration>(wc =>
                wc.Id == agentMessage.InboxId
                && wc.StorageConfiguration == storageConfiguration
                && wc.StorageType == storageType
                ), It.Is<WalletCredentials>(wc =>
                wc.Key == agentMessage.InboxKey
                && wc.StorageCredentials == storageCredentials
            )));
        }
    }
}
