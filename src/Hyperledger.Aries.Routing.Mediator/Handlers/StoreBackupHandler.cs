using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Routing.BackupRestore;
using Hyperledger.Aries.Routing.Mediator.Storage;
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

            var responseId = await _storageService.SaveWallet(msg.BackupId, msg.Payload);
    
            _eventAggregator.Publish(new StoreBackupResponseAgentMessage
            {
                BackupId = responseId
            });

            return null;
        }
    }
}