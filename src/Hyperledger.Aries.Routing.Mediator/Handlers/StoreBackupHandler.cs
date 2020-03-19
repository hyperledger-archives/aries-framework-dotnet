using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Routing.Mediator.Storage;
using Hyperledger.Indy.CryptoApi;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Routing
{
    public class DefaultStoreBackupHandler : MessageHandlerBase<StoreBackupAgentMessage>
    {
        private readonly IStorageService _storageService;
        private readonly IEventAggregator _eventAggregator;
        
        
        public DefaultStoreBackupHandler(IStorageService storageService, IEventAggregator eventAggregator)
        {
            _storageService = storageService;
            _eventAggregator = eventAggregator;
        }

        protected override async Task<AgentMessage> ProcessAsync(StoreBackupAgentMessage message, IAgentContext agentContext, UnpackedMessageContext messageContext)
        {
            var result = await Crypto.VerifyAsync(
                message.BackupId, 
                message.Payload.FirstOrDefault().Data.Base64.GetBytesFromBase64(),
                message.PayloadSignature.GetBytesFromBase64());

            if (!result)
            {
                throw new ArgumentException($"{nameof(result)} signed payload does not come from this key");
            }

            var backupDate = await _storageService.StoreBackupAsync(message.BackupId, message.Payload);

            return new StoreBackupResponseAgentMessage
            {
                BackupTimestamp = backupDate
            };
        }
    }
}