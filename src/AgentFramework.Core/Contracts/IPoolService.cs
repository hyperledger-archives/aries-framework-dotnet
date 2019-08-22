using System.Threading.Tasks;
using Hyperledger.Indy.PoolApi;

namespace AgentFramework.Core.Contracts
{
    /// <summary>
    /// Pool service.
    /// </summary>
    public interface IPoolService
    {
        /// <summary>
        /// Opens the pool configuration with the specified name and sets
        /// the node protocol version of the current process.
        /// </summary>
        /// <param name="poolName">Name of the pool configuration.</param>
        /// <param name="protocolVersion">The protocol version of the nodes.</param>
        /// <returns>
        /// A handle to the pool.
        /// </returns>
        Task<Pool> GetPoolAsync(string poolName, int protocolVersion);

        /// <summary>
        /// Opens the pool configuration with the specified name.
        /// </summary>
        /// <returns>The pool async.</returns>
        /// <param name="poolName">Pool name.</param>
        Task<Pool> GetPoolAsync(string poolName);

        /// <summary>
        /// Creates a pool configuration.
        /// </summary>
        /// <param name="poolName">The name of the pool configuration.</param>
        /// <param name="genesisFile">Genesis transaction file.</param>
        /// <returns>
        /// </returns>
        Task CreatePoolAsync(string poolName, string genesisFile);
    }
}
