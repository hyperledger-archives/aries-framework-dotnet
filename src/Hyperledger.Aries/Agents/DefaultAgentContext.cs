using System.Collections.Generic;
using Hyperledger.Aries.Ledger;
using Hyperledger.Indy.WalletApi;

namespace Hyperledger.Aries.Agents
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

        /// <summary>
        /// Gets or sets the configured agent for this context
        /// </summary>
        public IAgent Agent { get; set; }
    }
}
