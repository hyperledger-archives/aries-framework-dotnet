using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Extensions;
using IndyLedger = Hyperledger.Indy.LedgerApi.Ledger;
using Hyperledger.Indy.PoolApi;
using Newtonsoft.Json.Linq;

namespace Hyperledger.Aries.Ledger
{
    /// <inheritdoc />
    public class DefaultPoolService : IPoolService
    {
        /// <summary>Collection of active pool handles.</summary>
        protected static readonly ConcurrentDictionary<string, Pool> Pools =
            new ConcurrentDictionary<string, Pool>();

        /// <summary>
        /// Concurrent collection of txn author agreements
        /// </summary>
        /// <returns></returns>
        protected static readonly ConcurrentDictionary<string, IndyTaa> Taas =
            new ConcurrentDictionary<string, IndyTaa>();

        /// <summary>
        /// Concurrent collection of acceptance mechanisms lists
        /// </summary>
        /// <returns></returns>
        protected static readonly ConcurrentDictionary<string, IndyAml> Amls =
            new ConcurrentDictionary<string, IndyAml>();

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
            var poolConfig = new { genesis_txn = genesisFile }.ToJson();

            await Pool.CreatePoolLedgerConfigAsync(poolName, poolConfig);
        }

        /// <inheritdoc />
        public async Task<IndyTaa> GetTaaAsync(string poolName)
        {
            IndyTaa ParseTaa(string response)
            {
                var jresponse = JObject.Parse(response);
                if (jresponse["result"]["data"].HasValues)
                {
                    return new IndyTaa
                    {
                        Text = jresponse["result"]["data"]["text"].ToString(),
                        Version = jresponse["result"]["data"]["version"].ToString()
                    };
                }
                return null;
            };

            if (Taas.TryGetValue(poolName, out var taa))
            {
                return taa;
            }

            var pool = await GetPoolAsync(poolName, 2);
            var req = await IndyLedger.BuildGetTxnAuthorAgreementRequestAsync(null, null);
            var res = await IndyLedger.SubmitRequestAsync(pool, req);

            EnsureSuccessResponse(res);

            taa = ParseTaa(res);
            Taas.TryAdd(poolName, taa);
            return taa;
        }

        void EnsureSuccessResponse(string res)
        {
            var response = JObject.Parse(res);

            if (!response["op"].ToObject<string>().Equals("reply", StringComparison.OrdinalIgnoreCase))
                throw new AriesFrameworkException(ErrorCode.LedgerOperationRejected, "Ledger operation rejected");
        }

        /// <inheritdoc />
        public async Task<IndyAml> GetAmlAsync(string poolName, DateTimeOffset timestamp = default, string version = null)
        {
            IndyAml ParseAml(string response)
            {
                var jresponse = JObject.Parse(response);
                if (jresponse["result"]["data"].HasValues)
                {
                    return jresponse["result"]["data"].ToObject<IndyAml>();
                }
                return null;
            };

            if (Amls.TryGetValue(poolName, out var aml))
            {
                return aml;
            }

            var pool = await GetPoolAsync(poolName, 2);
            var req = await IndyLedger.BuildGetAcceptanceMechanismsRequestAsync(
                submitter_did: null,
                timestamp: timestamp == DateTimeOffset.MinValue ? -1 : timestamp.ToUnixTimeSeconds(),
                version: version);
            var res = await IndyLedger.SubmitRequestAsync(pool, req);

            EnsureSuccessResponse(res);

            aml = ParseAml(res);
            Amls.TryAdd(poolName, aml);
            return aml;
        }
    }
}