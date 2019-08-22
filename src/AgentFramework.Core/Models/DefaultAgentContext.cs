using System.Collections.Generic;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Messages;
using Hyperledger.Indy.WalletApi;

namespace AgentFramework.Core.Models
{
    /// <inheritdoc />
    /// <summary>
    /// Default Agent Context Object.
    /// </summary>
    public class DefaultAgentContext : IAgentContext
    {
        /// <inheritdoc />
        /// <summary>
        /// The agent context wallet,
        /// </summary>
        public Wallet Wallet { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// The agent context pool.
        /// </summary>
        public PoolAwaitable Pool { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// The agent context state.
        /// </summary>
        public Dictionary<string, string> State { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// A list of supported messages for the current agent.
        /// </summary>
        public IList<MessageType> SupportedMessages { get; set; }
    }
}
