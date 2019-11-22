using System;
using System.Threading.Tasks;
using Hyperledger.Aries.Ledger;
using Hyperledger.Indy.PoolApi;

namespace Hyperledger.Aries.Contracts
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
        /// Gets the transaction author agreement if one is set on
        /// the ledger. Otherwise, returns null.
        /// </summary>
        /// <param name="poolName"></param>
        /// <returns></returns>
        Task<IndyTaa> GetTaaAsync(string poolName);

        /// <summary>
        /// Gets the acceptance mechanisms list from the ledger
        /// if one is set.await Otherwise, returns null.
        /// </summary>
        /// <param name="poolName"></param>
        /// <param name="timestamp"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        Task<IndyAml> GetAmlAsync(string poolName, DateTimeOffset timestamp = default(DateTimeOffset), string version = null);

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
