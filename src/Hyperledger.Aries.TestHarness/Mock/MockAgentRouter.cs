using Hyperledger.Aries.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hyperledger.TestHarness.Mock
{
    public class MockAgentRouter
    {
        public void RegisterAgent(MockAgent agent)
        {
            Func<(string name, byte[] data), Task<MessageContext>> function = async (cb) => await agent.HandleInboundAsync(new PackedMessageContext(cb.data));
            _agentInBoundCallBacks.Add((agent.Name, function));
        }

        public Task<MessageContext> RouteMessage(string name, byte[] data)
        {
            var result = _agentInBoundCallBacks.FirstOrDefault(_ => _.name == name);
            return result.callback.Invoke((name,data));
        }

        private readonly List<(string name, Func<(string name, byte[] data), Task<MessageContext>> callback)> _agentInBoundCallBacks = new List<(string name, Func<(string name, byte[] data), Task<MessageContext>> callback)>();
    }
}
