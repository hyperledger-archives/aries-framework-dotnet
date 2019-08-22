using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Handlers;
using AgentFramework.Core.Messages;

namespace AgentFramework.Core.Tests
{
    public class MockMessageHandler : IMessageHandler
    {
        public IEnumerable<MessageType> SupportedMessageTypes { get; }
        public Task<AgentMessage> ProcessAsync(IAgentContext agentContext, MessageContext messageContext)
        {
            throw new NotImplementedException();
        }
    }
}
