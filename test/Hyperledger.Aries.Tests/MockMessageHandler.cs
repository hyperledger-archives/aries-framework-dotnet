using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;

namespace Hyperledger.Aries.Tests
{
    public class MockMessageHandler : IMessageHandler
    {
        public IEnumerable<MessageType> SupportedMessageTypes { get; }
        public Task<AgentMessage> ProcessAsync(IAgentContext agentContext, UnpackedMessageContext messageContext)
        {
            throw new NotImplementedException();
        }
    }
}
