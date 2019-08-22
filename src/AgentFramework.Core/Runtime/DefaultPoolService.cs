using System.Collections.Concurrent;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Extensions;
using Hyperledger.Indy.PoolApi;

namespace AgentFramework.Core.Runtime
{
    /// <inheritdoc />
    public class DefaultPoolService : IPoolService
    {
        /// <summary>Collection of active pool handles.</summary>
        protected static readonly ConcurrentDictionary<string, Pool> Pools =
            new ConcurrentDictionary<string, Pool>();

        /// <inheritdoc />
        public virtual async Task<Pool> GetPoolAsync(string poolName, int protocolVersion)
        {
            await Pool.SetProtocolVersionAsync(protocolVersion);

            return await GetPoolAsync(poolName);
        }

        /// <inheritdoc />
        public virtual async Task<Pool> GetPoolAsync(string poolName)
        {
            if (Pools.TryGetValue(poolName, out var pool))
            {
                return pool;
            }

            pool = await Pool.OpenPoolLedgerAsync(poolName, null);
            Pools.TryAdd(poolName, pool);
            return pool;
        }

        /// <inheritdoc />
        public virtual async Task CreatePoolAsync(string poolName, string genesisFile)
        {
            var poolConfig = new {genesis_txn = genesisFile}.ToJson();

            await Pool.CreatePoolLedgerConfigAsync(poolName, poolConfig);
        }
    }
}