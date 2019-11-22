﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hyperledger.Aries.Agents
{
    /// <summary>
    /// Message handler interface
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Gets the supported message types.
        /// </summary>
        /// <value>
        /// The supported message types.
        /// </value>
        IEnumerable<MessageType> SupportedMessageTypes { get; }

        /// <summary>
        /// Processes the agent message
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="messageContext">The agent message context.</param>
        /// <returns>Outgoing message context async.</returns>
        Task<AgentMessage> ProcessAsync(IAgentContext agentContext, UnpackedMessageContext messageContext);
    }
}