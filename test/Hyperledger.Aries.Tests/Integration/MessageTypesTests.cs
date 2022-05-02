using Hyperledger.Aries.Storage;
using Hyperledger.TestHarness.Mock;
using System;
using System.Threading.Tasks;
using Hyperledger.Aries.TestHarness;
using Xunit;

namespace Hyperledger.Aries.Tests.Integration
{
    public class MessageTypesTests : IAsyncLifetime
    {
        WalletConfiguration config1 = new WalletConfiguration { Id = Guid.NewGuid().ToString() };
        WalletConfiguration config2 = new WalletConfiguration { Id = Guid.NewGuid().ToString() };
        WalletCredentials cred = new WalletCredentials { Key = "2" };

        private MockAgent _agent1;
        private MockAgent _agent2;
        private readonly MockAgentRouter _router = new MockAgentRouter();

        public async Task InitializeAsync()
        {
            _agent1 = await MockUtils.CreateAsync("agent1", config1, cred, new MockAgentHttpHandler((cb) => _router.RouteMessage(cb.name, cb.data)), null, false);
            _router.RegisterAgent(_agent1);
            _agent2 = await MockUtils.CreateAsync("agent2", config2, cred, new MockAgentHttpHandler((cb) => _router.RouteMessage(cb.name, cb.data)), null, true);
            _router.RegisterAgent(_agent2);
        }

        [Fact]
        public void CanUseLegacyMessageType()
        {
            AgentScenarios.CreateBasicMessageWithLegacyType(_agent1);
        }

        [Fact]
        public void CanUseHttpsMessageType()
        {
            AgentScenarios.CreateBasicMessageWithHttpsType(_agent2);
        }

        public async Task DisposeAsync()
        {
            await _agent1.Dispose();
            await _agent2.Dispose();
        }
    }
}
