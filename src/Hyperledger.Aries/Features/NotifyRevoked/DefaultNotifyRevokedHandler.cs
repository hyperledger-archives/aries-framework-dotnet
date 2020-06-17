// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Hyperledger.Aries.Agents;
// using Hyperledger.Aries.Contracts;
// using Hyperledger.Aries.Decorators.Threading;
// using Hyperledger.Aries.Models.Events;

// namespace Hyperledger.Aries.Features.NotifyRevoked
// {
//     public class DefaultNotifyRevokedHandler : IMessageHandler
//     {
//         private readonly IEventAggregator _eventAggregator;

//         private readonly IMessageService _messageService;

//         public DefaultNotifyRevokedHandler(IEventAggregator eventAggregator, IMessageService messageService)
//         {
//             _eventAggregator = eventAggregator;
//             _messageService = messageService;
//         }

//         public IEnumerable<MessageType> SupportedMessageTypes => new[]
//         {
//             new MessageType(MessageTypes.NotifyRevoked)
//         };

//         public async Task<AgentMessage> ProcessAsync(IAgentContext agentContext, UnpackedMessageContext messageContext)
//         {
//             await Task.Yield();

//             switch (messageContext.GetMessageType())
//             {
//                 case 
//             }
//         }
//     }
// }