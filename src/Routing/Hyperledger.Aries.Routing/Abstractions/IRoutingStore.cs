using System;
using System.Threading.Tasks;

namespace Hyperledger.Aries.Routing
{
    public interface IRoutingStore
    {
        Task AddRouteAsync(string destinationRoute, string inboxId);

        Task<string> FindRouteAsync(string destinationRoute);
    }
}
