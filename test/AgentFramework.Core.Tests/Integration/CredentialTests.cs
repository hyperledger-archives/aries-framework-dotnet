using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgentFramework.Core.Models.Credentials;
using AgentFramework.Core.Models.Wallets;
using AgentFramework.TestHarness;
using AgentFramework.TestHarness.Mock;
using Xunit;

namespace AgentFramework.Core.Tests.Integration
{
    public class CredentialTests : IAsyncLifetime
    {
        WalletConfiguration config1 = new WalletConfiguration { Id = Guid.NewGuid().ToString() };
        WalletConfiguration config2 = new WalletConfiguration { Id = Guid.NewGuid().ToString() };
        WalletCredentials cred = new WalletCredentials { Key = "2" };

        private MockAgent _issuerAgent;
        private MockAgent _holderAgent;
        private readonly MockAgentRouter _router = new MockAgentRouter();

        public async Task InitializeAsync()
        {
            _issuerAgent = await MockUtils.CreateAsync("issuer", config1, cred, new MockAgentHttpHandler((cb) => _router.RouteMessage(cb.name, cb.data)), TestConstants.StewartDid);
            _router.RegisterAgent(_issuerAgent);
            _holderAgent = await MockUtils.CreateAsync("holder", config2, cred, new MockAgentHttpHandler((cb) => _router.RouteMessage((cb).name, cb.data)));
            _router.RegisterAgent(_holderAgent);
        }

        [Fact]
        public async Task CanIssueCredential()
        {
            (var issuerConnection, var holderConnection)  = await AgentScenarios.EstablishConnectionAsync(_issuerAgent, _holderAgent);
            await AgentScenarios.IssueCredentialAsync(_issuerAgent, _holderAgent, issuerConnection, holderConnection, new List<CredentialPreviewAttribute>
            {
                new CredentialPreviewAttribute("first_name", "Test"),
                new CredentialPreviewAttribute("last_name", "Holder")
            });
        }

        public async Task DisposeAsync()
        {
            await _issuerAgent.Dispose();
            await _holderAgent.Dispose();
        }
    }
}
