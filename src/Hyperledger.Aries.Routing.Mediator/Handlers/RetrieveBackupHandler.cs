using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Decorators.Attachments;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Routing.BackupRestore;
using Hyperledger.Aries.Routing.Mediator.Storage;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Routing
{
    public class RetrieveBackupHandler : IMessageHandler
    {
        private readonly IStorageService _storageService;
        private readonly IEventAggregator _eventAggregator;
        
        public IEnumerable<MessageType> SupportedMessageTypes => new MessageType[]
        {
            BackupTypeNames.RetrieveBackupAgentMessage,
            BackupTypeNames.RetrieveBackupResponseAgentMessage
        };

        public RetrieveBackupHandler(IStorageService storageService, IEventAggregator eventAggregator)
        {
            _storageService = storageService;
            _eventAggregator = eventAggregator;
        }
        
        public async Task<AgentMessage> ProcessAsync(IAgentContext agentContext, UnpackedMessageContext messageContext)
        {
            var msgJson = messageContext.GetMessageJson();
            var msg = JsonConvert.DeserializeObject<RetrieveBackupResponseAgentMessage>(msgJson);
            
            var bytesArray = await _storageService.RetrieveWallet(msg.BackupId);
            var payload = bytesArray.ToBase64String();
            
            _eventAggregator.Publish(new RetrieveBackupResponseAgentMessage
            {
                Payload = new[]
                {
                    new Attachment
                    {
                        Id = "libindy-backup-request-0",
                        MimeType = CredentialMimeTypes.ApplicationJsonMimeType,
                        Data = new AttachmentContent
                        {
                            Base64 = payload
                        }
                    }
                }
            });

            return null;
        }
    }
}