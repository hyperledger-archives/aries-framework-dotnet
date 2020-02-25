using System;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.Routing;
using Hyperledger.Aries.Routing;
using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Routing
{
    /// <summary>
    /// Mediator Forward Handler
    /// </summary>
    public class MediatorForwardHandler : MessageHandlerBase<ForwardMessage>
    {
        private readonly IWalletRecordService recordService;
        private readonly IWalletService walletService;
        private readonly IRoutingStore routingStore;
        private readonly IEventAggregator eventAggregator;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediatorForwardHandler"/> class.
        /// </summary>
        /// <param name="recordService"></param>
        /// <param name="walletService"></param>
        /// <param name="routingStore"></param>
        /// <param name="eventAggregator"></param>
        public MediatorForwardHandler(
            IWalletRecordService recordService,
            IWalletService walletService,
            IRoutingStore routingStore,
            IEventAggregator eventAggregator)
        {
            this.recordService = recordService;
            this.walletService = walletService;
            this.routingStore = routingStore;
            this.eventAggregator = eventAggregator;
        }

        /// <inheritdoc />
        protected override async Task<AgentMessage> ProcessAsync(ForwardMessage message, IAgentContext agentContext, UnpackedMessageContext messageContext)
        {
            var inboxId = await routingStore.FindRouteAsync(message.To);
            var inboxRecord = await recordService.GetAsync<InboxRecord>(agentContext.Wallet, inboxId);

            var edgeWallet = await walletService.GetWalletAsync(inboxRecord.WalletConfiguration, inboxRecord.WalletCredentials);

            var inboxItemRecord = new InboxItemRecord { ItemData = message.Message.ToJson(), Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds() };
            await recordService.AddAsync(edgeWallet, inboxItemRecord);

            eventAggregator.Publish(new InboxItemEvent
            {
                InboxId = inboxId,
                ItemId = inboxItemRecord.Id
            });

            return null;
        }
    }
}
