using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Decorators.Attachments;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Routing.BackupRestore;
using Hyperledger.Aries.Routing.Mediator.Storage;
using Hyperledger.Indy.CryptoApi;
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
            BackupTypeNames.RetrieveBackupResponseAgentMessage,
            BackupTypeNames.ListBackupsAgentMessage
        };

        public RetrieveBackupHandler(IStorageService storageService, IEventAggregator eventAggregator)
        {
            _storageService = storageService;
            _eventAggregator = eventAggregator;
        }
        
        public async Task<AgentMessage> ProcessAsync(IAgentContext agentContext, UnpackedMessageContext messageContext)
        {
            var msgJson = messageContext.GetMessageJson();

            switch (messageContext.GetMessageType())
            {
                case BackupTypeNames.RetrieveBackupAgentMessage:
                {
                    var msg = JsonConvert.DeserializeObject<RetreiveBackupAgentMessage>(msgJson);

                    var result = await Crypto.VerifyAsync(msg.BackupId, msg.BackupId.GetBytesFromBase64(), msg.Signature.GetBytesFromBase64());

                    if (!result)
                    {
                        throw new ArgumentException($"{nameof(result)} signature does not match the signer");
                    }
                    
                    var bytesArray = await _storageService.RetrieveBackupAsync(msg.BackupId, msg.DateTimeOffset);

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
                    
                    break;
                }
                case BackupTypeNames.ListBackupsAgentMessage:
                {
                    var msg = JsonConvert.DeserializeObject<ListBackupsAgentMessage>(msgJson);
                    var backupList = await _storageService.ListBackupsAsync(msg.BackupId);
                    
                    _eventAggregator.Publish(new ListBackupsResponseAgentMessage()
                    {
                        BackupList = backupList
                    });
                    
                    break;
                }
            }

            return null;
        }
    }
}