using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hyperledger.Aries.Agents
{
    /// <summary>
    /// Agent.
    /// </summary>
    public interface IAgent
    {
        /// <summary>
        /// Gets the service provider used by this agent instance
        /// </summary>
        IServiceProvider Provider { get; }

        /// <summary>
        /// Processes the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="context">Context.</param>
        /// <param name="messageContext">Message context.</param>
        Task<MessageContext> ProcessAsync(IAgentContext context, MessageContext messageContext);

        /// <summary>
        /// Gets the handlers.
        /// </summary>
        /// <value>The handlers.</value>
        IList<IMessageHandler> Handlers { get; }
    }
}
