using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.IssueCredential.Models.Messages;
using Hyperledger.Aries.Features.IssueCredential.Models;
using Hyperledger.Aries.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Options;

namespace Hyperledger.Aries.Features.IssueCredential
{
    internal class DefaultCredentialHandler : IMessageHandler
    {
        private readonly AgentOptions _agentOptions;
        private readonly ICredentialService _credentialService;
        private readonly IWalletRecordService _recordService;
        private readonly IMessageService _messageService;

        /// <summary>Initializes a new instance of the <see cref="DefaultCredentialHandler"/> class.</summary>
        /// <param name="agentOptions">The agent options.</param>
        /// <param name="credentialService">The credential service.</param>
        /// <param name="recordService">The wallet record service.</param>
        /// <param name="messageService">The message service.</param>
        public DefaultCredentialHandler(
            IOptions<AgentOptions> agentOptions,
            ICredentialService credentialService,
            IWalletRecordService recordService,
            IMessageService messageService)
        {
            _agentOptions = agentOptions.Value;
            _credentialService = credentialService;
            _recordService = recordService;
            _messageService = messageService;
        }

        /// <summary>
        /// Gets the supported message types.
        /// </summary>
        /// <value>
        /// The supported message types.
        /// </value>
        public IEnumerable<MessageType> SupportedMessageTypes => new MessageType[]
        {
            MessageTypes.IssueCredentialNames.AcknowledgeCredential,
            MessageTypes.IssueCredentialNames.OfferCredential,
            MessageTypes.IssueCredentialNames.RequestCredential,
            MessageTypes.IssueCredentialNames.IssueCredential,
            MessageTypesHttps.IssueCredentialNames.AcknowledgeCredential,
            MessageTypesHttps.IssueCredentialNames.OfferCredential,
            MessageTypesHttps.IssueCredentialNames.RequestCredential,
            MessageTypesHttps.IssueCredentialNames.IssueCredential
        };

        /// <summary>
        /// Processes the agent message
        /// </summary>
        /// <param name="agentContext"></param>
        /// <param name="messageContext">The agent message.</param>
        /// <returns></returns>
        /// <exception cref="AriesFrameworkException">Unsupported message type {messageType}</exception>
        public async Task<AgentMessage> ProcessAsync(IAgentContext agentContext, UnpackedMessageContext messageContext)
        {
            switch (messageContext.GetMessageType())
            {
                // v1
                case MessageTypesHttps.IssueCredentialNames.AcknowledgeCredential:
                case MessageTypes.IssueCredentialNames.AcknowledgeCredential:
                {
                    var acknowledgementMessage = messageContext.GetMessage<CredentialAcknowledgeMessage>();
                    await _credentialService.ProcessAcknowledgementMessageAsync(agentContext, acknowledgementMessage);
                    return null;
                }
                
                case MessageTypesHttps.IssueCredentialNames.OfferCredential:
                case MessageTypes.IssueCredentialNames.OfferCredential:
                    {
                        var offer = messageContext.GetMessage<CredentialOfferMessage>();
                        var recordId = await _credentialService.ProcessOfferAsync(
                            agentContext, offer, messageContext.Connection);

                        messageContext.ContextRecord = await _credentialService.GetAsync(agentContext, recordId);

                        // Auto request credential if set in the agent option
                        if (_agentOptions.AutoRespondCredentialOffer)
                        {
                            var (message, record) = await _credentialService.CreateRequestAsync(agentContext, recordId);
                            messageContext.ContextRecord = record;
                            return message;
                        }

                        return null;
                    }

                case MessageTypesHttps.IssueCredentialNames.RequestCredential:
                case MessageTypes.IssueCredentialNames.RequestCredential:
                    {
                        var request = messageContext.GetMessage<CredentialRequestMessage>();
                        var recordId = await _credentialService.ProcessCredentialRequestAsync(
                                agentContext: agentContext,
                                credentialRequest: request,
                                connection: messageContext.Connection);
                        if (request.ReturnRoutingRequested() && messageContext.Connection == null)
                        {
                            var (message, record) = await _credentialService.CreateCredentialAsync(agentContext, recordId);
                            messageContext.ContextRecord = record;
                            return message;
                        }
                        else
                        {
                            // Auto create credential if set in the agent option
                            if (_agentOptions.AutoRespondCredentialRequest == true)
                            {
                                var (message, record) = await _credentialService.CreateCredentialAsync(agentContext, recordId);
                                messageContext.ContextRecord = record;
                                return message;
                            }
                            messageContext.ContextRecord = await _credentialService.GetAsync(agentContext, recordId);
                            return null;
                        }
                    }

                case MessageTypesHttps.IssueCredentialNames.IssueCredential:
                case MessageTypes.IssueCredentialNames.IssueCredential:
                    {
                        var credential = messageContext.GetMessage<CredentialIssueMessage>();
                        var recordId = await _credentialService.ProcessCredentialAsync(
                            agentContext, credential, messageContext.Connection);

                        messageContext.ContextRecord = await UpdateValuesAsync(
                            credentialId: recordId,
                            credentialIssue: messageContext.GetMessage<CredentialIssueMessage>(),
                            agentContext: agentContext);

                        return null;
                    }
                default:
                    throw new AriesFrameworkException(ErrorCode.InvalidMessage,
                        $"Unsupported message type {messageContext.GetMessageType()}");
            }
        }

        private async Task<CredentialRecord> UpdateValuesAsync(string credentialId, CredentialIssueMessage credentialIssue, IAgentContext agentContext)
        {
            var credentialAttachment = credentialIssue.Credentials.FirstOrDefault(x => x.Id == "libindy-cred-0")
                ?? throw new ArgumentException("Credential attachment not found");

            var credentialJson = credentialAttachment.Data.Base64.GetBytesFromBase64().GetUTF8String();

            var jcred = JObject.Parse(credentialJson);
            var values = jcred["values"].ToObject<Dictionary<string, AttributeValue>>();

            var credential = await _credentialService.GetAsync(agentContext, credentialId);
            credential.CredentialAttributesValues = values.Select(x => new CredentialPreviewAttribute { Name = x.Key, Value = x.Value.Raw, MimeType = CredentialMimeTypes.TextMimeType }).ToList();
            await _recordService.UpdateAsync(agentContext.Wallet, credential);

            return credential;
        }

        private class AttributeValue
        {
            [JsonProperty("raw")]
            public string Raw { get; set; }

            [JsonProperty("encoded")]
            public string Encoded { get; set; }
        }
    }
}
