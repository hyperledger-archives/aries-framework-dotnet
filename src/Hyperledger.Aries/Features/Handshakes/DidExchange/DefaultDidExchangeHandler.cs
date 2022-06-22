using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Decorators.Threading;
using Hyperledger.Aries.Features.Handshakes.DidExchange.Models;

namespace Hyperledger.Aries.Features.Handshakes.DidExchange
{
    public class DefaultDidExchangeHandler : IMessageHandler
    {
        private readonly IDidExchangeService _didExchangeService;

        public DefaultDidExchangeHandler(IDidExchangeService didExchangeService)
        {
            _didExchangeService = didExchangeService;
        }

        public IEnumerable<MessageType> SupportedMessageTypes => new MessageType[]
        {
            MessageTypesHttps.DidExchange.Request,
            MessageTypesHttps.DidExchange.Response,
            MessageTypesHttps.DidExchange.Complete,
            MessageTypesHttps.DidExchange.ProblemReport
        };
        public async Task<AgentMessage> ProcessAsync(IAgentContext agentContext, UnpackedMessageContext messageContext)
        {
            switch (messageContext.GetMessageType())
            {
                case MessageTypesHttps.DidExchange.Request:
                    var request = messageContext.GetMessage<DidExchangeRequestMessage>();
                    try
                    {
                        await _didExchangeService.ProcessRequestAsync(agentContext, request, messageContext.Connection);
                    }
                    catch (Exception)
                    {
                        return CreateProblemReportMessage(request);
                    }
                    
                    return null;
                
                case MessageTypesHttps.DidExchange.Response:
                    var response = messageContext.GetMessage<DidExchangeResponseMessage>();
                    try
                    {
                        await _didExchangeService.ProcessResponseAsync(agentContext, response, messageContext.Connection);
                    }
                    catch (Exception)
                    {
                        return CreateProblemReportMessage(response);
                    }
                    
                    return null;
                
                case MessageTypesHttps.DidExchange.Complete:
                    var complete = messageContext.GetMessage<DidExchangeCompleteMessage>();
                    await _didExchangeService.ProcessComplete(agentContext, complete, messageContext.Connection);
                    return null;
                
                case MessageTypesHttps.DidExchange.ProblemReport:
                    var problemReport = messageContext.GetMessage<DidExchangeProblemReportMessage>();
                    await _didExchangeService.ProcessProblemReportMessage(agentContext, problemReport, messageContext.Connection);
                    return null;
                
                default:
                    throw new AriesFrameworkException(ErrorCode.InvalidMessage,
                        $"Unsupported message type {messageContext.GetMessageType()}");
            }
        }

        private static DidExchangeProblemReportMessage CreateProblemReportMessage(AgentMessage message)
        {
            var response = message.CreateThreadedReply<DidExchangeProblemReportMessage>();
            response.ProblemCode = message is DidExchangeRequestMessage
                ? DidExchangeProblemReportMessage.Error.RequestProcessingError
                : DidExchangeProblemReportMessage.Error.ResponseProcessingError;

            return response;
        }
    }
}
