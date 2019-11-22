﻿using System.IO;
using System.Threading.Tasks;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Ledger;
using Hyperledger.Indy.PoolApi;

namespace Hyperledger.TestHarness.Utils
{
    public class PoolUtils
    {
        private static IPoolService poolService = new DefaultPoolService();
        private static Pool pool;

        public static async Task<Pool> GetPoolAsync()
        {
            if (pool != null)
            {
                return pool;
            }

            try
            {
                await poolService.CreatePoolAsync("LocalTestPool", Path.GetFullPath("pool_genesis.txn"));
            }
            catch (PoolLedgerConfigExistsException)
            {
                // OK
            }
            return pool = await poolService.GetPoolAsync("LocalTestPool", 2);
        }
    }
}
