using System;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Decorators.Signature;
using AgentFramework.Core.Handlers.Agents;
using AgentFramework.Core.Models.Connections;
using Hyperledger.Indy.CryptoApi;
using Hyperledger.Indy.WalletApi;
using Xunit;

namespace AgentFramework.Core.Tests.Decorators
{
    public class SignatorDecoratorTests : IAsyncLifetime
    {
        private readonly string _walletConfig = $"{{\"id\":\"{Guid.NewGuid()}\"}}";
        private const string Credentials = "{\"key\":\"test_wallet_key\"}";
        private IAgentContext _agent;

        public async Task InitializeAsync()
        {
            try
            {
                await Wallet.CreateWalletAsync(_walletConfig, Credentials);
            }
            catch (WalletExistsException)
            {
                // OK
            }
            
            _agent = new AgentContext
            {
                Wallet = await Wallet.OpenWalletAsync(_walletConfig, Credentials),
            };
        }

        public async Task DisposeAsync()
        {
            if (_agent != null) await _agent.Wallet.CloseAsync();
            await Wallet.DeleteWalletAsync(_walletConfig, Credentials);
        }

        [Fact]
        public async Task CanSignData()
        {
            var data = new Connection
            {
                Did = "test"
            };

            var key = await Crypto.CreateKeyAsync(_agent.Wallet, "{}");

            var sigData = await SignatureUtils.SignData(_agent, data, key);
            
            Assert.True(sigData.SignatureType == SignatureUtils.DefaultSignatureType);
            Assert.NotNull(sigData.Signature);
            Assert.NotNull(sigData.SignatureData);
            Assert.NotNull(sigData.Signer);
        }

        [Fact]
        public async Task CanSignAndVerifyData()
        {
            var data = new Connection
            {
                Did = "test"
            };

            var key = await Crypto.CreateKeyAsync(_agent.Wallet, "{}");

            var sigData = await SignatureUtils.SignData(_agent, data, key);
            
            var result = SignatureUtils.UnpackAndVerifyData<Connection>(sigData);

            Assert.True(data.Did == result.Did);
        }
    }
}
