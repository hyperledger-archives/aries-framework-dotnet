using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Storage;
using Hyperledger.Aries.TestHarness;
using Hyperledger.TestHarness;
using Hyperledger.TestHarness.Mock;
using Xunit;

namespace Hyperledger.Aries.Tests.Integration
{
    public class CredentialTests : IAsyncLifetime
    {
        static CredentialTests()
        {
            global::Hyperledger.Aries.Utils.Runtime.SetFlags(Hyperledger.Aries.Utils.Runtime.LedgerLookupRetryFlag);
        }

        WalletConfiguration config1 = new WalletConfiguration { Id = Guid.NewGuid().ToString() };
        WalletConfiguration config2 = new WalletConfiguration { Id = Guid.NewGuid().ToString() };
        WalletCredentials cred = new WalletCredentials { Key = "2" };

        private MockAgent _issuerAgent;
        private MockAgent _holderAgent;
        private readonly MockAgentRouter _router = new MockAgentRouter();

        public async Task InitializeAsync()
        {
            _issuerAgent = await MockUtils.CreateAsync("issuer", config1, cred, new MockAgentHttpHandler((cb) => _router.RouteMessage(cb.name, cb.data)), TestConstants.StewardSeed);
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
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task CanIssueCredentialConnectionless(bool useDidKeyFormat)
        {
            await AgentScenarios.IssueCredentialConnectionlessAsync(_issuerAgent, _holderAgent, new List<CredentialPreviewAttribute>
            {
                new CredentialPreviewAttribute("first_name", "Test"),
                new CredentialPreviewAttribute("last_name", "Holder")
            }, useDidKeyFormat);
        }

        public async Task DisposeAsync()
        {
            await _issuerAgent.Dispose();
            await _holderAgent.Dispose();
        }
    }
}
