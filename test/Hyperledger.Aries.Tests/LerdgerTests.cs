using System.Threading.Tasks;
using Hyperledger.Aries.Contracts;
using Hyperledger.TestHarness;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace Hyperledger.Aries.Tests
{
    public class LedgerTests : TestSingleWallet
    {
        //protected override string GetPoolName() => "sovrin-builder";
        
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
    }
}