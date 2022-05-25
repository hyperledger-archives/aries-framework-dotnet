using System;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Features.Discovery;
using Hyperledger.Indy.WalletApi;
using Hyperledger.TestHarness.Utils;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Hyperledger.Aries.Tests.Protocols
{
    public class DiscoveryTests : IAsyncLifetime
    {
        private readonly string HolderConfiguration = $"{{\"id\":\"{Guid.NewGuid()}\"}}";
        private const string Credentials = "{\"key\":\"test_wallet_key\"}";

        private IAgentContext _holderContext;

        private readonly IDiscoveryService _discoveryService;

        public DiscoveryTests()
        {
            _discoveryService = new DefaultDiscoveryService(new Mock<IEventAggregator>().Object, new Mock<ILogger<DefaultDiscoveryService>>().Object);
        }

        public async Task InitializeAsync()
        {
            _holderContext = await AgentUtils.Create(HolderConfiguration, Credentials);
        }

        [Fact]
        public void CreateDiscoveryQueryThrowsArguementNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _discoveryService.CreateQuery(_holderContext, null));
            Assert.Throws<ArgumentNullException>(() => _discoveryService.CreateQuery(_holderContext, ""));
        }

        [Fact]
        public void CanCreateDiscoveryQuery()
        {
            var query = "did:sov:123456789abcdefghi1234;spec/*";
            var msg = _discoveryService.CreateQuery(_holderContext, query);

            Assert.NotNull(msg);
            Assert.True(msg.Query == query);
        }

        [Fact]
        public void CreateDiscoveryQueryResponseThrowsArguementNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _discoveryService.CreateQueryResponse(_holderContext, new Features.Discovery.DiscoveryQueryMessage()));
        }

        [Fact]
        public void CanPerformDiscoveryFlowWithWildcardQuery()
        {
            var query = "*";
            var msg = _discoveryService.CreateQuery(_holderContext, query);

            Assert.NotNull(msg);
            Assert.True(msg.Query == query);

            var rsp = _discoveryService.CreateQueryResponse(_holderContext, msg);

            Assert.Equal(5, rsp.Protocols.Count);
        }

        [Fact]
        public void CanPerformDiscoveryFlowWithFilteredQuery()
        {
            var query = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/connections/*";
            var msg = _discoveryService.CreateQuery(_holderContext, query);

            Assert.NotNull(msg);
            Assert.True(msg.Query == query);

            var rsp = _discoveryService.CreateQueryResponse(_holderContext, msg);

            Assert.True(rsp.Protocols.Count == 1);
        }

        public async Task DisposeAsync()
        {
            if (_holderContext != null) await _holderContext.Wallet.CloseAsync();

            await Wallet.DeleteWalletAsync(HolderConfiguration, Credentials);
        }
    }
}
