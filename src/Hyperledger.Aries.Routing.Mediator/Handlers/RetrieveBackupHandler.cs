using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Decorators.Attachments;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Routing.Mediator.Storage;
using Hyperledger.Indy.CryptoApi;
using Multiformats.Base;
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
                        var message = messageContext.GetMessage<RetrieveBackupAgentMessage>();

                        var signature = message.Signature.GetBytesFromBase64();
                        

                        // message.BackupId = public key
                        // message.BackupId.GetBytesFromBase64() => decoded as bytes
                        // message.Signature.GetBytesFromBase64() => decoded as bytes
                        
                        var result = await Crypto.VerifyAsync(
                            message.BackupId,
                            message.BackupId.GetBytesFromBase64(),
                            signature);


                        if (!result)
                        {
                            throw new ArgumentException($"{nameof(result)} signature does not match the signer");
                        }

                        var backupAttachments = await _storageService.RetrieveBackupAsync(message.BackupId);
                        return new RetrieveBackupResponseAgentMessage
                        {
                            Payload = backupAttachments
                        };
                    }
                case BackupTypeNames.ListBackupsAgentMessage:
                    {
                        var message = messageContext.GetMessage<ListBackupsAgentMessage>();
                        var backupList = await _storageService.ListBackupsAsync(message.BackupId);

                        return new ListBackupsResponseAgentMessage
                        {
                            BackupList = backupList
                                .Select(x => long.Parse(x))
                                .Select(x => DateTimeOffset.FromUnixTimeSeconds(x))
                                .ToList()
                        };
                    }
            }

            return null;
        }
    }
}