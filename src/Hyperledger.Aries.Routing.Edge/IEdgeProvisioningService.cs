using System.Threading;
using System.Threading.Tasks;
using Hyperledger.Aries.Configuration;

namespace Hyperledger.Aries.Agents.Edge
{
    public interface IEdgeProvisioningService
    {
        Task ProvisionAsync(AgentOptions options, CancellationToken cancellationToken = default);
        Task ProvisionAsync(CancellationToken cancellationToken = default);
    }
}