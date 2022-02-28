using System;
using System.Threading.Tasks;
using Hyperledger.Aries.Contracts;
using Hyperledger.TestHarness;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace Hyperledger.Aries.Tests
{
    public class LedgerTests : TestSingleWallet
    {
        protected override string GetIssuerSeed() => TestConstants.StewardSeed;

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
        
        [Fact(DisplayName = "Set and get service endpoint on ledger")]
        public async Task SetAndGetServiceEndpointFromLedger()
        {
            var endpoint = $"http://{Guid.NewGuid().ToString().ToLowerInvariant()}";

            await ledgerService.RegisterServiceEndpointAsync(Context, TestConstants.StewardDid, endpoint);

            var result = await ledgerService.LookupServiceEndpointAsync(Context, TestConstants.StewardDid);
            
            Assert.Equal(endpoint, result.Result.Endpoint);
        }
    }
}