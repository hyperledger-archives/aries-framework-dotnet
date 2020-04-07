using System;
using System.Collections.Generic;
using System.IO;
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
                        var backupId = Multibase.Base58.Decode(message.BackupId);

                        var result = await Crypto.VerifyAsync(
                            message.BackupId,
                            backupId,
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
                        var timestampList = backupList.Select(p => new DirectoryInfo(p).Name);

                        return new ListBackupsResponseAgentMessage
                        {
                            BackupList = timestampList
                                .Select(x => long.Parse(x))
                                .OrderByDescending(x => x)
                                .ToList()
                        };
                    }
            }

            return null;
        }
    }
}