using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Polly;

namespace Hyperledger.Aries.Utils
{
    internal class ResilienceUtils
    {
        internal static T RetryPolicy<T, E>(Func<T> action, Func<E, bool> exceptionPredicate = null)
            where E : Exception
        {
            if (Runtime.Flags.Contains(Runtime.LedgerLookupRetryFlag))
            {
                return Policy.Handle(exceptionPredicate)
                    .WaitAndRetry(3, x => TimeSpan.FromSeconds(x * 2))
                    .Execute(action);
            }
            return Policy.NoOp<T>().Execute(action);
        }

        internal static Task<T> RetryPolicyAsync<T, E>(Func<Task<T>> action, Func<E, bool> exceptionPredicate = null)
            where E : Exception
        {
            if (Runtime.Flags.Contains(Runtime.LedgerLookupRetryFlag))
            {
                return Policy.Handle(exceptionPredicate)
                    .WaitAndRetryAsync(3, x => TimeSpan.FromSeconds(x * 2))
                    .ExecuteAsync(action);
            }
            return Policy.NoOpAsync<T>().ExecuteAsync(action);
        }
    }

    /// <summary>
    /// Runtime configuration
    /// </summary>
    public static class Runtime
    {
        internal static IReadOnlyList<string> Flags { get; set; } = new List<string>().AsReadOnly();

        /// <summary>
        /// Enables ledger lookup retry policy
        /// </summary>
        public const string LedgerLookupRetryFlag = "EnableLedgerLookupRetryPolicy";

        /// <summary>
        /// Sets runtime flags used to configure some services behavior.
        /// </summary>
        /// <param name="flags"></param>
        public static void SetFlags(params string[] flags)
        {
            Flags = new List<string>(flags).AsReadOnly();
        }
    }
}
