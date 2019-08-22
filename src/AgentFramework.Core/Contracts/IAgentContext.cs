using System.Collections.Generic;
using AgentFramework.Core.Messages;
using AgentFramework.Core.Models;
using Hyperledger.Indy.WalletApi;

namespace AgentFramework.Core.Contracts
{
    /// <summary>
    /// Represents an agent context
    /// </summary>
    public interface IAgentContext
    {
        /// <summary>Gets or sets the agent wallet.</summary>
        /// <value>The wallet.</value>
        Wallet Wallet { get; set; }

        /// <summary>Gets or sets the pool.</summary>
        /// <value>The pool.</value>
        PoolAwaitable Pool { get; set; }

        /// <summary>Name/value utility store to pass data
        /// along the execution pipeline.</summary>
        /// <value>The state.</value>
        Dictionary<string, string> State { get; set; }

        /// <summary>
        /// Gets or sets the supported messages of the current agent.
        /// </summary>
        IList<MessageType> SupportedMessages { get; set; }
    }
}