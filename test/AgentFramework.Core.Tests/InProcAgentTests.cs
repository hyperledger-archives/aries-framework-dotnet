using System.Threading.Tasks;
using AgentFramework.Core.Models.Records;
using AgentFramework.TestHarness.Mock;
using Xunit;

namespace AgentFramework.Core.Tests
{
    public class InProcAgentTests
    {
        [Fact(DisplayName = "Create two InProc agents and establish connection")]
        public async Task CreateInProcAgentsAndConnect()
        {
            var agents = await InProcAgent.CreatePairedAsync(true);

            Assert.NotNull(agents.Agent1);
            Assert.NotNull(agents.Agent2);

            Assert.Equal(ConnectionState.Connected, agents.Connection1.State);
            Assert.Equal(ConnectionState.Connected, agents.Connection2.State);

            await agents.Agent1.DisposeAsync();
            await agents.Agent2.DisposeAsync();
        }
    }
}
