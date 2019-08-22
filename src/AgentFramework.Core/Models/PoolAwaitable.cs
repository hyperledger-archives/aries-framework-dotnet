using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Hyperledger.Indy.PoolApi;

namespace AgentFramework.Core.Models
{
    /// <summary>
    /// Awaitable pool handle.
    /// </summary>
    public struct PoolAwaitable
    {
        private readonly Func<Task<Pool>> _initializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolAwaitable"/> struct.
        /// </summary>
        /// <param name="initializer">Initializer.</param>
        public PoolAwaitable(Func<Task<Pool>> initializer)
        {
            _initializer = initializer;
        }

        /// <summary>
        /// Gets the awaiter for this instance.
        /// </summary>
        /// <returns>The awaiter.</returns>
        public TaskAwaiter<Pool> GetAwaiter()
        {
            return _initializer().GetAwaiter();
        }

        /// <summary>
        /// Create new <see cref="PoolAwaitable"/> instance from existing <see cref="Pool"/> handle
        /// </summary>
        /// <returns>The pool awatable.</returns>
        /// <param name="pool">Pool.</param>
        public static PoolAwaitable FromPool(Pool pool)
        {
            return new PoolAwaitable(() => Task.FromResult(pool));
        }
    }
}
