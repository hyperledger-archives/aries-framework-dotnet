using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Features.Handshakes.Common;
using Hyperledger.Aries.Features.Handshakes.Connection;
using Hyperledger.Aries.Features.Handshakes.Connection.Models;
using Hyperledger.Aries.Storage;
using Microsoft.Extensions.Logging;

namespace Hyperledger.Aries.Routing
{
    internal class EdgeConnectionService : DefaultConnectionService
    {
        private readonly IEdgeClientService edgeClientService;

        public EdgeConnectionService(
            IEdgeClientService edgeClientService,
            IEventAggregator eventAggregator, 
            IWalletRecordService recordService, 
            IProvisioningService provisioningService, 
            ILogger<DefaultConnectionService> logger) 
            : base(eventAggregator, recordService, provisioningService, logger)
        {
            this.edgeClientService = edgeClientService;
        }

        /// <inheritdoc />
        public override async Task<(ConnectionRequestMessage, ConnectionRecord)> CreateRequestAsync(IAgentContext agentContext, ConnectionInvitationMessage invitation)
        {
            var (message, record) = await base.CreateRequestAsync(agentContext, invitation);

            await edgeClientService.AddRouteAsync(agentContext, record.MyVk);

            return (message, record);
        }

        /// <inheritdoc />
        public override async Task<(ConnectionResponseMessage, ConnectionRecord)> CreateResponseAsync(IAgentContext agentContext, string connectionId)
        {
            var (message, record) = await base.CreateResponseAsync(agentContext, connectionId);

            await edgeClientService.AddRouteAsync(agentContext, record.MyVk);

            return (message, record);
        }

        /// <inheritdoc />
        public override async Task<(ConnectionInvitationMessage, ConnectionRecord)> CreateInvitationAsync(IAgentContext agentContext, InviteConfiguration config = null)
        {
            var (message, record) = await base.CreateInvitationAsync(agentContext, config);

            await edgeClientService.AddRouteAsync(agentContext, message.RecipientKeys.First());

            return (message, record);
        }
    }
}
