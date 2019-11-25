using System.Threading;
using System.Threading.Tasks;

namespace Hyperledger.Aries.Agents.Edge
{
    public interface IEdgeProvsioningService
    {
        Task ProvisionAsync(CancellationToken cancellationToken = default);
    }
}