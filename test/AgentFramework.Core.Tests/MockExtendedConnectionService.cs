using System.Collections.Generic;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Messages.Connections;
using AgentFramework.Core.Models.Connections;
using AgentFramework.Core.Models.Records;
using AgentFramework.Core.Models.Records.Search;

namespace AgentFramework.Core.Tests
{
    public class MockExtendedConnectionService : IConnectionService
    {
        public Task<ConnectionRecord> GetAsync(IAgentContext agentContext, string connectionId)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<ConnectionRecord>> ListAsync(IAgentContext agentContext, ISearchQuery query = null, int count = 100)
        {
            throw new System.NotImplementedException();
        }

        public Task<(ConnectionInvitationMessage, ConnectionRecord)> CreateInvitationAsync(IAgentContext agentContext, InviteConfiguration config = null)
        {
            throw new System.NotImplementedException();
        }

        public Task RevokeInvitationAsync(IAgentContext agentContext, string invitationId)
        {
            throw new System.NotImplementedException();
        }

        public Task<(ConnectionRequestMessage, ConnectionRecord)> CreateRequestAsync(IAgentContext agentContext, ConnectionInvitationMessage offer)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> ProcessRequestAsync(IAgentContext agentContext, ConnectionRequestMessage request, ConnectionRecord connection)
        {
            throw new System.NotImplementedException();
        }

        public Task<(ConnectionResponseMessage, ConnectionRecord)> CreateResponseAsync(IAgentContext agentContext, string connectionId)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> DeleteAsync(IAgentContext agentContext, string connectionId)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> ProcessResponseAsync(IAgentContext agentContext, ConnectionResponseMessage response, ConnectionRecord connection)
        {
            throw new System.NotImplementedException();
        }
    }
}