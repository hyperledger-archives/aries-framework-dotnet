using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.TestHarness;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Hyperledger.Indy.LedgerApi;
using System;

namespace AgentFramework.Core.Tests
{
    public class LedgerTests : TestSingleWallet
    {
        //protected override string GetPoolName() => "sovrin-builder";
        
        [Fact(DisplayName = "Get Transaction Author Agreement from ledger if exists")]
        public async Task GetTaaFromLedger()
        {
            var taa = await Host.Services.GetService<ILedgerService>()
                .LookupTaaAsync(Context);

            var aml = await Host.Services.GetService<ILedgerService>()
                .LookupAmlAsync(Context);

            Assert.True(true);
        }

        [Fact(DisplayName = "Get Acceptance Mechanisms List from ledger if exists")]
        public async Task GetAmlFromLedger()
        {
            var aml = await Host.Services.GetService<ILedgerService>()
                .LookupAmlAsync(Context);

            Assert.True(true);
        }
    }
}