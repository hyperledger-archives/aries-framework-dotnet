using System.Threading.Tasks;

namespace Hyperledger.Aries.Agents
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
        Task OnMessageAsync(IAgentContext agentContext, UnpackedMessageContext messageContext);
    }
}
