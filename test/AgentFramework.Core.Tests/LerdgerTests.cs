using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.TestHarness;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace AgentFramework.Core.Tests
{
    public class LedgerTests : TestSingleWallet
    {
        //protected override string GetPoolName() => "sovrin-builder";
        
        [Fact(DisplayName = "Get TAA from ledger if exists")]
        public async Task GetTaaFromLedger()
        {
            var taa = await Host.Services.GetService<ILedgerService>()
                .LookupTaaAsync(Context);

            Assert.True(true);
        }
    }
}