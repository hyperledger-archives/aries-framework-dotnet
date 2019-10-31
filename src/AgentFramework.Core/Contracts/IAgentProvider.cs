using System.Threading.Tasks;
using AgentFramework.Core.Contracts;

namespace AgentFramework.Core.Handlers.Agents
{
    /// <summary>
    /// Agent Context Provider.
    /// </summary>
    public interface IAgentProvider
    {
        /// <summary>
        /// Retrieves an agent context.
        /// </summary>
        /// <param name="args">Arguments</param>
        /// <returns>The agent context async.</returns>
        Task<IAgentContext> GetContextAsync(params object[] args);

        /// <summary>
        /// Retrieves an agent instance.
        /// </summary>
        /// <returns>The agent async.</returns>
        /// <param name="args">Arguments.</param>
        Task<IAgent> GetAgentAsync(params object[] args);
    }
}