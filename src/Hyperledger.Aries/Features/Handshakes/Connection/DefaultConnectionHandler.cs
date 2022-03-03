using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Features.Handshakes.Connection.Models;
using Hyperledger.Aries.Utils;

namespace Hyperledger.Aries.Features.Handshakes.Connection
{
    internal class DefaultConnectionHandler : IMessageHandler
    {
        private readonly IConnectionService _connectionService;

        /// <summary>Initializes a new instance of the <see cref="DefaultConnectionHandler"/> class.</summary>
        /// <param name="connectionService">The connection service.</param>
        public DefaultConnectionHandler(
            IConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        /// <inheritdoc />
        /// <summary>
        /// Gets the supported message types.
        /// </summary>
        /// <value>
        /// The supported message types.
        /// </value>
        public IEnumerable<MessageType> SupportedMessageTypes => new MessageType[]
        {
            MessageTypes.ConnectionAcknowledgement, 
            MessageTypes.ConnectionInvitation,
            MessageTypes.ConnectionRequest,
            MessageTypes.ConnectionResponse,
            MessageTypesHttps.ConnectionAcknowledgement,
            MessageTypesHttps.ConnectionInvitation,
            MessageTypesHttps.ConnectionRequest,
            MessageTypesHttps.ConnectionResponse
        };

        /// <summary>
        /// Processes the agent message
        /// </summary>
        /// <param name="agentContext"></param>
        /// <param name="messageContext">The agent message agentContext.</param>
        /// <returns></returns>
        /// <exception cref="AriesFrameworkException">Unsupported message type {message.Type}</exception>
        public async Task<AgentMessage> ProcessAsync(IAgentContext agentContext, UnpackedMessageContext messageContext)
        {
            switch (messageContext.GetMessageType())
            {
                case MessageTypesHttps.ConnectionAcknowledgement:
                case MessageTypes.ConnectionAcknowledgement:
                    {
                        var acknowledgementMessage = messageContext.GetMessage<ConnectionAcknowledgeMessage>();
                        await _connectionService.ProcessAcknowledgementMessageAsync(agentContext, acknowledgementMessage);
                        return null;
                    }
                
                case MessageTypesHttps.ConnectionInvitation:
                case MessageTypes.ConnectionInvitation:
                    {
                        var invitation = messageContext.GetMessage<ConnectionInvitationMessage>();
                        await _connectionService.ProcessInvitationAsync(agentContext, invitation);
                        return null;
                    }

                case MessageTypesHttps.ConnectionRequest:
                case MessageTypes.ConnectionRequest:
                    {
                        var request = messageContext.GetMessage<ConnectionRequestMessage>();
                        var connectionId = await _connectionService.ProcessRequestAsync(agentContext, request, messageContext.Connection);
                        messageContext.ContextRecord = messageContext.Connection;

                        // Auto accept connection if set during invitation
                        if (messageContext.Connection.GetTag(TagConstants.AutoAcceptConnection) == "true")
                        {
                            var (message, record) = await _connectionService.CreateResponseAsync(agentContext, connectionId);
                            messageContext.ContextRecord = record;
                            return message;
                        }
                        return null;
                    }

                case MessageTypesHttps.ConnectionResponse:
                case MessageTypes.ConnectionResponse:
                    {
                        var response = messageContext.GetMessage<ConnectionResponseMessage>();
                        await _connectionService.ProcessResponseAsync(agentContext, response, messageContext.Connection);
                        messageContext.ContextRecord = messageContext.Connection;
                        return null;
                    }
                default:
                    throw new AriesFrameworkException(ErrorCode.InvalidMessage,
                        $"Unsupported message type {messageContext.GetMessageType()}");
            }
        }
    }
}
