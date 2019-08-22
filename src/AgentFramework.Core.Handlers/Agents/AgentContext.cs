using System.Collections.Concurrent;
using AgentFramework.Core.Messages;
using AgentFramework.Core.Models;

namespace AgentFramework.Core.Handlers.Agents
{
    /// <summary>
    /// Agent context that represents the context of a current agent.
    /// </summary>
    public class AgentContext : DefaultAgentContext
    {
        private readonly ConcurrentQueue<MessageContext> _queue = new ConcurrentQueue<MessageContext>();
        
        /// <summary>
        /// Adds a message to the current processing queue
        /// </summary>
        /// <param name="message"></param>
        public void AddNext(MessageContext message) => _queue.Enqueue(message);

        internal bool TryGetNext(out MessageContext message) => _queue.TryDequeue(out message);
    }
}