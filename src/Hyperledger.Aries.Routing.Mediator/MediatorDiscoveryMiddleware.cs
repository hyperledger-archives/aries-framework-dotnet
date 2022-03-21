using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Agents.Edge;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.Handshakes.Connection;
using Hyperledger.Aries.Features.Handshakes.Connection.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Hyperledger.Aries.Routing
{
    public class MediatorDiscoveryMiddleware : IMiddleware
    {
        private readonly AgentOptions options;
        private readonly IProvisioningService provisioningService;
        private readonly IConnectionService connectionService;
        private readonly IAgentProvider agentProvider;

        public MediatorDiscoveryMiddleware(
            IOptions<AgentOptions> options,
            IProvisioningService provisioningService,
            IConnectionService connectionService,
            IAgentProvider agentProvider)
        {
            this.options = options.Value;
            this.provisioningService = provisioningService;
            this.connectionService = connectionService;
            this.agentProvider = agentProvider;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var agentConfiguration = await GetConfigurationAsync();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            await context.Response.WriteAsync(agentConfiguration.ToJson());
        }

        public async Task<AgentPublicConfiguration> GetConfigurationAsync()
        {
            var agentContext = await agentProvider.GetContextAsync();
            var provisioningRecord = await provisioningService.GetProvisioningAsync(agentContext.Wallet);
            var connectionId = provisioningRecord.GetTag(MediatorProvisioningService.EdgeInvitationTagName);

            if (connectionId == null)
            {
                throw new Exception("This agent hasn't been provisioned as mediator agent");
            }
            var inviation = await connectionService.GetAsync(agentContext, connectionId);

            var agentConfiguration = new AgentPublicConfiguration
            {
                ServiceEndpoint = provisioningRecord.Endpoint.Uri,
                RoutingKey = provisioningRecord.Endpoint.Verkey.First(),
                Invitation = inviation.GetTag(MediatorProvisioningService.InvitationTagName)
                    .ToObject<ConnectionInvitationMessage>()
            };
            return agentConfiguration;
        }
    }
}
