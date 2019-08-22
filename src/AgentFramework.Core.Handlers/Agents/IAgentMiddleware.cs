using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Messages;

namespace AgentFramework.Core.Handlers.Agents
{
    /// <summary>
    /// Agent middleware used to process a message after the message handler processing
    /// </summary>
    public interface IAgentMiddleware
    {
        /// <summary>
        /// Called when the message needs to be processed
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="messageContext">The message context.</param>
        /// <returns></returns>
        Task OnMessageAsync(IAgentContext agentContext, MessageContext messageContext);
    }
}
