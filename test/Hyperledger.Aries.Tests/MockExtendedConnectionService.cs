using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Common;
using Hyperledger.Aries.Features.Handshakes.Common;
using Hyperledger.Aries.Features.Handshakes.Connection;
using Hyperledger.Aries.Features.Handshakes.Connection.Models;
using Hyperledger.Aries.Features.OutOfBand;
using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Tests
{
    public class MockExtendedConnectionService : IConnectionService
    {
        public Task<ConnectionRecord> GetAsync(IAgentContext agentContext, string connectionId)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<ConnectionRecord>> ListAsync(IAgentContext agentContext, ISearchQuery query = null, int count = 100, int skip = 0)
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

        public Task<ConnectionRecord> ProcessInvitationAsync(IAgentContext agentContext, ConnectionInvitationMessage offer)
        {
            throw new System.NotImplementedException();
        }

        public Task<ConnectionRecord> ProcessInvitationAsync(IAgentContext agentContext, InvitationMessage invitation)
        {
            throw new System.NotImplementedException();
        }

        public Task<(ConnectionRequestMessage, ConnectionRecord)> CreateRequestAsync(IAgentContext agentContext, ConnectionRecord connectionRecord)
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

        public Task<ConnectionAcknowledgeMessage> CreateAcknowledgementMessageAsync(IAgentContext agentContext, string connectionRecordId,
            string status = AcknowledgementStatusConstants.Ok)
        {
            throw new System.NotImplementedException();
        }

        public Task<ConnectionRecord> ProcessAcknowledgementMessageAsync(IAgentContext agentContext,
            ConnectionAcknowledgeMessage connectionAcknowledgeMessage)
        {
            throw new System.NotImplementedException();
        }

        public Task<ConnectionRecord> ResolveByMyKeyAsync(IAgentContext agentContext, string myKey)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> ProcessResponseAsync(IAgentContext agentContext, ConnectionResponseMessage response, ConnectionRecord connection)
        {
            throw new System.NotImplementedException();
        }
    }
}
