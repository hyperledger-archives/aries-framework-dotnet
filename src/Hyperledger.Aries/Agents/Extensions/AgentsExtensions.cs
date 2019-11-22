using System.Collections.Generic;
using System.Linq;

namespace Hyperledger.Aries.Agents
{
    /// <summary>Agents Extensions</summary>
    public static class AgentsExtensions
    {
        /// <summary>Wraps the message in payload.</summary>
        /// <param name="agentMessage">The agent message.</param>
        /// <param name="senderKey"></param>
        /// <returns></returns>
        public static UnpackedMessageContext AsMessageContext(this AgentMessage agentMessage, string senderKey) =>
            new UnpackedMessageContext(agentMessage, senderKey);

        internal static AgentContext AsAgentContext(this IAgentContext context)
        {
            return new AgentContext
            {
                Wallet = context.Wallet,
                Pool = context.Pool,
                SupportedMessages = context.SupportedMessages,
                State = context.State,
                Agent = context.Agent
            };
        }

        /// <summary>
        /// Gets the supported message types.
        /// </summary>
        /// <returns>The supported message types.</returns>
        /// <param name="agent">Agent.</param>
        public static IList<MessageType> GetSupportedMessageTypes(this IAgent agent) => 
            agent.Handlers.SelectMany(x => x.SupportedMessageTypes).ToList();
    }
}