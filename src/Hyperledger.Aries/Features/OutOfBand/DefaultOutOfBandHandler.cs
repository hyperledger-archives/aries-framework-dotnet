using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;

namespace Hyperledger.Aries.Features.OutOfBand
{
    public class DefaultOutOfBandHandler : IMessageHandler
    {
        private readonly IOutOfBandService _outOfBandService;

        public DefaultOutOfBandHandler(IOutOfBandService outOfBandService)
        {
            _outOfBandService = outOfBandService;
        }

        public IEnumerable<MessageType> SupportedMessageTypes => new MessageType[]
        {
            MessageTypesHttps.OutOfBand.Invitation,
            MessageTypesHttps.OutOfBand.HandshakeReuse,
            MessageTypesHttps.OutOfBand.HandshakeReuseAccepted,
            MessageTypesHttps.OutOfBand.ProblemReport
        };
        
        public async Task<AgentMessage> ProcessAsync(IAgentContext agentContext, UnpackedMessageContext messageContext)
        {
            switch (messageContext.GetMessageType())
            {
                case MessageTypesHttps.OutOfBand.HandshakeReuse:
                    var reuseMessage = messageContext.GetMessage<HandshakeReuseMessage>();
                    var msg = await _outOfBandService.ProcessHandshakeReuseMessage(agentContext, reuseMessage);
                    
                    return msg;
                
                case MessageTypesHttps.OutOfBand.HandshakeReuseAccepted:
                    var reuseAccepted = messageContext.GetMessage<HandshakeReuseAcceptedMessage>();
                    await _outOfBandService.ProcessHandshakeReuseAccepted(agentContext, reuseAccepted);
                    return null;
                
                case MessageTypesHttps.OutOfBand.ProblemReport:
                    throw new NotImplementedException();

                default:
                    throw new AriesFrameworkException(ErrorCode.InvalidMessage,
                        $"Unsupported message type {messageContext.GetMessageType()}");
            }
        }
    }
}
