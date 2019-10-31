using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Messages;

namespace AgentFramework.Core.Handlers.Agents
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
        Task<MessageResponse> ProcessAsync(IAgentContext context, MessageContext messageContext);

        /// <summary>
        /// Gets the handlers.
        /// </summary>
        /// <value>The handlers.</value>
        IList<IMessageHandler> Handlers { get; }
    }
}