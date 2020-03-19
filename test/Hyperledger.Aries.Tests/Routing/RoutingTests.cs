using Hyperledger.Aries.Agents.Edge;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Features.DidExchange;
using Hyperledger.TestHarness.Mock;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Hyperledger.Aries.Tests.Routing
{

    /// <summary>
    /// Routing Tests
    /// </summary>
    public class RoutingTests
    {
        [Fact(DisplayName = "Provision and connect a mediator and edge agent")]
        public async Task CreatePairedAgentsWithRouting()
        {
            var pair = await InProcAgent.CreatePairedWithRoutingAsync();

            var connections1 = await pair.Agent1.Connections.ListAsync(pair.Agent1.Context);
            var invitation1 = connections1.FirstOrDefault(x => x.State == ConnectionState.Invited);

            var connection1 = connections1.FirstOrDefault(x => x.Id != invitation1.Id);
            var connection2 = (await pair.Agent2.Connections.ListAsync(pair.Agent2.Context)).FirstOrDefault();

            var provisioning1 = await pair.Agent1.Host.Services.GetRequiredService<IProvisioningService>()
                .GetProvisioningAsync(pair.Agent1.Context.Wallet);
            var provisioning2 = await pair.Agent2.Host.Services.GetRequiredService<IProvisioningService>()
                .GetProvisioningAsync(pair.Agent2.Context.Wallet);

            // Connections exist
            Assert.NotNull(invitation1);
            Assert.NotNull(connection1);
            Assert.NotNull(connection2);

            // The two connections are connected in the correct state
            Assert.Equal(ConnectionState.Connected, connection1.State);
            Assert.Equal(ConnectionState.Connected, connection2.State);

            // Check mediator and edge provisioning record states
            Assert.Equal(provisioning1.GetTag(MediatorProvisioningService.EdgeInvitationTagName), invitation1.Id);
            Assert.Equal(provisioning2.GetTag(EdgeProvisioningService.MediatorConnectionIdTagName), connection2.Id);
        }
    }
}
