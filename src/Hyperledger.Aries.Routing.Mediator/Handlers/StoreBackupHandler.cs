using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Routing.BackupRestore;
using Hyperledger.Aries.Routing.Mediator.Storage;
using Hyperledger.Indy.CryptoApi;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Routing
{
    public class DefaultStoreBackupHandler : IMessageHandler
    {
        private readonly IStorageService _storageService;
        private readonly IEventAggregator _eventAggregator;
        
        public IEnumerable<MessageType> SupportedMessageTypes => new MessageType[]
        {
            BackupTypeNames.StoreBackupAgentMessage,
        };
        
        public DefaultStoreBackupHandler(IStorageService storageService, IEventAggregator eventAggregator)
        {
            _storageService = storageService;
            _eventAggregator = eventAggregator;
        }

        public async Task<AgentMessage> ProcessAsync(IAgentContext agentContext, UnpackedMessageContext messageContext)
        {
            if (!SupportedMessageTypes.Contains(messageContext.GetMessageType()))
                return null;
            
            var msgJson = messageContext.GetMessageJson();
            var msg = JsonConvert.DeserializeObject<StoreBackupAgentMessage>(msgJson);

            var result = await Crypto.VerifyAsync(msg.BackupId, msg.Payload.FirstOrDefault().Data.Base64.GetBytesFromBase64(), msg.PayloadSignature.GetBytesFromBase64());

            if (!result)
            {
                throw new ArgumentException($"{nameof(result)} signed payload does not come from this key");
            }
            
            var responseId = await _storageService.StoreBackupAsync(msg.BackupId, msg.Payload);
    
            _eventAggregator.Publish(new StoreBackupResponseAgentMessage
            {
                BackupId = responseId,
                Timestamp = msg.Timestamp
            });

            return null;
        }
    }
}