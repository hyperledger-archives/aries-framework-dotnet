using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Attachments.Abstractions;
using Hyperledger.Aries.Attachments.Records;
using Hyperledger.Aries.Decorators.Attachments;
using Hyperledger.Aries.Storage;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hyperledger.Aries.Attachments
{
    public class DefaultAttachmentService : IAttachmentService
    {
        private readonly IWalletRecordService _recordService;

        public DefaultAttachmentService(IWalletRecordService recordService)
        {
            _recordService = recordService;
        }

        public async Task<string> Create(IAgentContext context, AgentMessage message, string recordId, string nickname)
        {
            var attachment = message.GetAttachment(nickname);
            await _recordService.AddAsync(context.Wallet, new AttachmentRecord
            {
                Id = attachment.Id,
                Data = attachment.Data,
                Filename = attachment.Filename,
                MimeType = attachment.MimeType,
                Nickname = attachment.Nickname,
                RecordId = recordId
            });
            return attachment.Id;
        }

        public async Task<AttachmentRecord> GetAsync(IAgentContext agentContext, string id)
        {
            return await _recordService.GetAsync<AttachmentRecord>(agentContext.Wallet, id) ??
                   throw new AriesFrameworkException(ErrorCode.RecordNotFound, "Attachment record not found");
        }

        public async Task<AttachmentRecord> GetByRecordIdAsync(IAgentContext agentContext, string id)
        {
            var request = SearchQuery.Equal(nameof(AttachmentRecord.RecordId), id);
            var result = await _recordService.SearchAsync<AttachmentRecord>(agentContext.Wallet, request);
            var attachment = result.FirstOrDefault();
            return  attachment ?? throw new AriesFrameworkException(ErrorCode.RecordNotFound, "Attachment record not found");
        }
    }
}
