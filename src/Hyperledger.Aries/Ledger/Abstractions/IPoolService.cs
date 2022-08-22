using System;
using System.Threading.Tasks;
using Hyperledger.Aries.Ledger;

namespace Hyperledger.Aries.Contracts
{
    /// <summary>
    /// Pool service.
    /// </summary>
    public interface IPoolService
    {
        /// <summary>
        /// Gets the pool with the specified name and sets
        /// the node protocol version of the current process.
        /// </summary>
        /// <param name="poolName">Name of the pool configuration.</param>
        /// <param name="protocolVersion">The protocol version of the nodes.</param>
        /// <returns>
        /// A handle to the pool.
        /// </returns>
        Task<object> GetPoolAsync(string poolName, int protocolVersion);

        /// <summary>
        /// Gets the pool configuration with the specified name.
        /// </summary>
        /// <returns>A handle to the pool.</returns>
        /// <param name="poolName">Pool name.</param>
        Task<object> GetPoolAsync(string poolName);

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
        /// Creates a pool for a given genesis file.
        /// </summary>
        /// <param name="poolName">The name of the pool configuration.</param>
        /// <param name="genesisFile">Genesis transaction file.</param>
        /// <returns>
        /// </returns>
        Task CreatePoolAsync(string poolName, string genesisFile);
        
        /// <summary>
        /// Submit a ledger request for a given pool.
        /// </summary>
        /// <param name="poolHandle">The awaitable pool handle.</param>
        /// <param name="requestHandle">The request handle.</param>
        /// <returns></returns>
        public Task<string> SubmitRequestAsync(PoolAwaitable poolHandle, object requestHandle);
    }
}
