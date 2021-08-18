using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Decorators.Threading;
using Hyperledger.Aries.Features.DidExchange;
using Hyperledger.Aries.Features.Statistic.Messages;
using Hyperledger.Aries.Features.Statistic.Models;
using Hyperledger.Aries.Storage;
using System;
using System.Threading.Tasks;

namespace Hyperledger.Aries.Features.Statistic
{
    public class DefaultStatisticService : IStatisticService
    {
        private readonly IConnectionService ConnectionService;
        private readonly IWalletRecordService RecordService;
        private readonly IMessageService MessageService;

        public DefaultStatisticService(
            IConnectionService connectionService,
            IWalletRecordService recordService,
            IMessageService messageService
        )
        {
            ConnectionService = connectionService;
            RecordService = recordService;
            MessageService = messageService;
        }

        public async Task<PresentProofStatisticMessage> CreatePresentProof(IAgentContext agentContext, PresentProofRequest presentProof, string connectionId)
        {
            if (connectionId != null)
            {
                var connection = await ConnectionService.GetAsync(agentContext, connectionId);

                if (connection.State != ConnectionState.Connected)
                    throw new AriesFrameworkException(ErrorCode.RecordInInvalidState,
                        $"Connection state was invalid. Expected '{ConnectionState.Connected}', found '{connection.State}'");
            }

            var threadId = Guid.NewGuid().ToString();
            var message = new PresentProofStatisticMessage
            {
                Id = threadId,
                CredentialDefinitions = presentProof.CredentialDefinitions,
                HolderDid = presentProof.HolderDid,
                ProofId = presentProof.ProofId,
                VerifierDid = presentProof.VerifierDid
            };
            message.ThreadFrom(threadId);
            return message;
        }

        public async Task<PresentProofRecord> ProcessPresentProof(IAgentContext context, PresentProofStatisticMessage message, ConnectionRecord connection)
        {
            var presentProof = new PresentProofRecord
            {
                Id = Guid.NewGuid().ToString(),
                ProofId = message.ProofId,
                CredentialDefinitions = message.CredentialDefinitions,
                HolderDid = message.HolderDid,
                VerifierDid = message.VerifierDid
            };
            await RecordService.AddAsync(context.Wallet, presentProof);
            return presentProof;
        }
    }
}
