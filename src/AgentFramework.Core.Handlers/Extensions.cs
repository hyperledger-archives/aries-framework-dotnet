using System.Collections.Generic;
using System.Linq;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Handlers.Agents;
using AgentFramework.Core.Handlers.Internal;
using AgentFramework.Core.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace AgentFramework.Core.Handlers
{
    /// <summary>Extensions</summary>
    public static class Extensions
    {
        /// <summary>Wraps the message in payload.</summary>
        /// <param name="agentMessage">The agent message.</param>
        /// <returns></returns>
        public static MessageContext AsMessageContext(this AgentMessage agentMessage) =>
            new MessageContext(agentMessage);

        /// <summary>Adds the default message handlers.</summary>
        /// <param name="collection">The collection.</param>
        public static void AddDefaultMessageHandlers(this IServiceCollection collection)
        {
            collection.AddTransient<DefaultConnectionHandler>();
            collection.AddTransient<DefaultCredentialHandler>();
            collection.AddTransient<DefaultProofHandler>();
            collection.AddTransient<DefaultForwardHandler>();
            collection.AddTransient<DefaultTrustPingMessageHandler>();
            collection.AddTransient<DefaultDiscoveryHandler>();
            collection.AddTransient<DefaultEphemeralChallengeHandler>();
            collection.AddTransient<DefaultBasicMessageHandler>();
        }

        internal static AgentContext AsAgentContext(this IAgentContext context)
        {
            return new AgentContext
            {
                Wallet = context.Wallet,
                Pool = context.Pool,
                SupportedMessages = context.SupportedMessages,
                State = context.State
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