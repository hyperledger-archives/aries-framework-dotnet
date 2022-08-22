using System.Threading.Tasks;
using Hyperledger.Aries.Contracts;
using Hyperledger.TestHarness;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Hyperledger.Aries.Tests
{
    public abstract class PoolServiceTests : TestSingleWallet
    {
        protected TestSingleWallet _fixture;
        
        [Fact(DisplayName = "Get Transaction Author Agreement from ledger if exists")]
        public async Task GetTaaFromLedger()
        {
            var taa = await Host.Services.GetService<IPoolService>()
                .GetTaaAsync(GetPoolName());

            var aml = await Host.Services.GetService<IPoolService>()
                .GetAmlAsync(GetPoolName());

            Assert.True(true);
        }

        [Fact(DisplayName = "Get Acceptance Mechanisms List from ledger if exists")]
        public async Task GetAmlFromLedger()
        {
            var aml = await Host.Services.GetService<IPoolService>()
                .GetAmlAsync(GetPoolName());

            Assert.True(true);
        }
        
        public class PoolServiceV1Tests : PoolServiceTests, IClassFixture<PoolServiceV1Tests.SingleTestWalletFixture>
        {
            public class SingleTestWalletFixture : TestSingleWallet
            {
            }
        
            public PoolServiceV1Tests(SingleTestWalletFixture fixture)
            {
                _fixture = fixture;
            }
        }

        public class PoolServiceV2Tests : PoolServiceTests, IClassFixture<PoolServiceV2Tests.SingleTestWalletV2Fixture>
        {
            public class SingleTestWalletV2Fixture : TestSingleWalletV2
            {
            }
        
            public PoolServiceV2Tests(SingleTestWalletV2Fixture fixture)
            {
                _fixture = fixture;
            }
        }
    }
}