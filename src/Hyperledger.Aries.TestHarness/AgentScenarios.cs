using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Decorators.Threading;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.BasicMessage;
using Hyperledger.Aries.Features.Discovery;
using Hyperledger.Aries.Features.Handshakes.Common;
using Hyperledger.Aries.Features.Handshakes.Connection;
using Hyperledger.Aries.Features.Handshakes.Connection.Models;
using Hyperledger.Aries.Features.Handshakes.DidExchange;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Features.OutOfBand;
using Hyperledger.Aries.Features.PresentProof;
using Hyperledger.Aries.Models.Events;
using Hyperledger.Aries.Storage;
using Hyperledger.Aries.Utils;
using Hyperledger.TestHarness;
using Hyperledger.TestHarness.Mock;
using Hyperledger.TestHarness.Utils;
using Newtonsoft.Json;
using Xunit;

namespace Hyperledger.Aries.TestHarness
{
    public static class AgentScenarios
    {
        public static async Task<(ConnectionRecord inviteeConnection,ConnectionRecord inviterConnection)> EstablishConnectionAsync(MockAgent invitee, MockAgent inviter, bool useDidKeyFormat = false)
        {
            var slim = new SemaphoreSlim(0, 1);
            
            var connectionService = invitee.GetService<IConnectionService>();
            var messageService = invitee.GetService<IMessageService>();

            // Hook into response message event of second runtime to release semaphore
            inviter.GetService<IEventAggregator>().GetEventByType<ServiceMessageProcessingEvent>()
                .Where(x => x.MessageType == MessageTypes.ConnectionResponse)
                .Subscribe(x => slim.Release());

            (var invitation, var inviterConnection) = await connectionService.CreateInvitationAsync(invitee.Context,
                new InviteConfiguration { AutoAcceptConnection = true, UseDidKeyFormat = useDidKeyFormat});

            var (request, inviteeConnection) =
                await connectionService.CreateRequestAsync(inviter.Context, invitation);
            await messageService.SendAsync(inviter.Context, request, inviteeConnection);

            // Wait for connection to be established or continue after 30 sec timeout
            await slim.WaitAsync(TimeSpan.FromSeconds(30));

            var connectionRecord1 = await connectionService.GetAsync(invitee.Context, inviterConnection.Id);
            var connectionRecord2 = await connectionService.GetAsync(inviter.Context, inviteeConnection.Id);

            Assert.Equal(ConnectionState.Connected, connectionRecord1.State);
            Assert.Equal(ConnectionState.Connected, connectionRecord2.State);
            Assert.Equal(connectionRecord1.MyDid, connectionRecord2.TheirDid);
            Assert.Equal(connectionRecord1.TheirDid, connectionRecord2.MyDid);

            Assert.Equal(
                connectionRecord1.GetTag(TagConstants.LastThreadId),
                connectionRecord2.GetTag(TagConstants.LastThreadId));

            return (connectionRecord1, connectionRecord2);
        }

        public static async Task<(ConnectionRecord requesterConnection, ConnectionRecord responderConnection)> ExchangeDidsWithPrivateDids(MockAgent requester, MockAgent responder)
        {
            var outOfBandService = requester.GetService<IOutOfBandService>();
            var didExchangeService = requester.GetService<IDidExchangeService>();
            var connectionService = requester.GetService<IConnectionService>();
            var messsageService = requester.GetService<IMessageService>();

            var slim = new SemaphoreSlim(0, 1);
            responder.GetService<IEventAggregator>().GetEventByType<ServiceMessageProcessingEvent>()
                .Where(x => x.MessageType == MessageTypesHttps.DidExchange.Request)
                .Subscribe(x => slim.Release());

            var (invitation, responderRecord) = await outOfBandService.CreateInvitationAsync(responder.Context, new List<AgentMessage>());

            var (_, requesterRecord) = await outOfBandService.ProcessInvitationMessage(requester.Context, invitation);

            var (requestMessage, _) = await didExchangeService.CreateRequestAsync(requester.Context, requesterRecord);
            requesterRecord = await connectionService.GetAsync(requester.Context, requesterRecord.Id);
            
            Assert.Equal(invitation.Id, requestMessage.GetParentThreadId());
            Assert.Equal(invitation.Id, requesterRecord.GetTag(TagConstants.ParentThreadId));
            Assert.Equal(requestMessage.Id, requesterRecord.GetTag(TagConstants.LastThreadId));

            await messsageService.SendAsync(requester.Context, requestMessage, requesterRecord);
            
            await slim.WaitAsync(TimeSpan.FromSeconds(30));

            requesterRecord = await connectionService.GetAsync(requester.Context, requesterRecord.Id);
            responderRecord = await connectionService.GetAsync(responder.Context, responderRecord.Id);
            Assert.Equal(ConnectionState.Negotiating, responderRecord.State);
            Assert.Equal(ConnectionState.Negotiating, requesterRecord.State);
            Assert.Equal(ConnectionRole.Invitee, requesterRecord.Role);
            Assert.Equal(ConnectionRole.Inviter, responderRecord.Role);
            Assert.Equal(invitation.Id, requesterRecord.GetTag(TagConstants.ParentThreadId));
            Assert.Equal(invitation.Id, responderRecord.GetTag(TagConstants.ParentThreadId));
            Assert.Equal(requestMessage.Id, requesterRecord.GetTag(TagConstants.LastThreadId));
            Assert.Equal(requestMessage.Id, responderRecord.GetTag(TagConstants.LastThreadId));
            
            requester.GetService<IEventAggregator>().GetEventByType<ServiceMessageProcessingEvent>()
                .Where(x => x.MessageType == MessageTypesHttps.DidExchange.Response)
                .Subscribe(x => slim.Release());

            var (responseMessage, _) = await didExchangeService.CreateResponseAsync(responder.Context, responderRecord);
            responderRecord = await connectionService.GetAsync(responder.Context, responderRecord.Id);

            await messsageService.SendAsync(responder.Context, responseMessage, responderRecord);
            
            await slim.WaitAsync(TimeSpan.FromSeconds(30));
            
            requesterRecord = await connectionService.GetAsync(requester.Context, requesterRecord.Id);
            responderRecord = await connectionService.GetAsync(responder.Context, responderRecord.Id);
            Assert.Equal(ConnectionState.Connected, responderRecord.State);
            Assert.Equal(ConnectionState.Connected, requesterRecord.State);
            Assert.Equal(ConnectionRole.Invitee, requesterRecord.Role);
            Assert.Equal(ConnectionRole.Inviter, responderRecord.Role);
            
            responder.GetService<IEventAggregator>().GetEventByType<ServiceMessageProcessingEvent>()
                .Where(x => x.MessageType == MessageTypesHttps.DidExchange.Complete)
                .Subscribe(x => slim.Release());

            var (completeMessage, _) = await didExchangeService.CreateComplete(requester.Context, requesterRecord);

            await messsageService.SendAsync(requester.Context, completeMessage, requesterRecord);
            
            await slim.WaitAsync(TimeSpan.FromSeconds(30));
            
            requesterRecord = await connectionService.GetAsync(requester.Context, requesterRecord.Id);
            responderRecord = await connectionService.GetAsync(responder.Context, responderRecord.Id);
            Assert.Equal(ConnectionState.Connected, responderRecord.State);
            Assert.Equal(ConnectionState.Connected, requesterRecord.State);
            Assert.Equal(ConnectionRole.Invitee, requesterRecord.Role);
            Assert.Equal(ConnectionRole.Inviter, responderRecord.Role);
            
            Assert.Equal(requesterRecord.TheirDid, responderRecord.MyDid);
            Assert.Equal(requesterRecord.TheirVk, responderRecord.MyVk);
            
            Assert.Equal(requesterRecord.MyDid, responderRecord.TheirDid);
            Assert.Equal(requesterRecord.MyVk, responderRecord.TheirVk);

            return (requesterRecord, responderRecord);
        }

        public static async Task<(ConnectionRecord requesterConnection, ConnectionRecord responderConnection)> ExchangeDidsWithPublicDid(MockAgent requester, MockAgent responder)
        {
            var slim = new SemaphoreSlim(0, 1);
            
            var outOfBandService = requester.GetService<IOutOfBandService>();
            var didExchangeService = requester.GetService<IDidExchangeService>();
            var connectionService = requester.GetService<IConnectionService>();
            var messsageService = requester.GetService<IMessageService>();

            var (invitation, responderRecord) = await outOfBandService.CreateInvitationAsync(responder.Context, config: new InviteConfiguration {UsePublicDid = true});
            
            var (_, requesterRecord) = await outOfBandService.ProcessInvitationMessage(requester.Context, invitation);
            
            var (requestMessage, _) = await didExchangeService.CreateRequestAsync(requester.Context, requesterRecord);
            requesterRecord = await connectionService.GetAsync(requester.Context, requesterRecord.Id);
            
            Assert.Equal(invitation.Id, requestMessage.GetParentThreadId());

            // Did Exchange Request
            responder.GetService<IEventAggregator>().GetEventByType<ServiceMessageProcessingEvent>()
                .Where(x => x.MessageType == MessageTypesHttps.DidExchange.Request)
                .Subscribe(x => slim.Release());

            await messsageService.SendAsync(requester.Context, requestMessage, requesterRecord);
            
            await slim.WaitAsync(TimeSpan.FromSeconds(30));

            requesterRecord = await connectionService.GetAsync(requester.Context, requesterRecord.Id);
            responderRecord = (await connectionService.ListAsync(responder.Context, SearchQuery.Equal(nameof(ConnectionRecord.TheirDid), requesterRecord.MyDid))).First();

            Assert.Equal(ConnectionState.Negotiating, responderRecord.State);
            Assert.Equal(ConnectionState.Negotiating, requesterRecord.State);
            Assert.Equal(requesterRecord.TheirDid,  DidUtils.ToDid(DidUtils.DidSovMethodSpec, TestConstants.StewardDid));
            Assert.Equal(responderRecord.TheirDid, requesterRecord.MyDid);

            Assert.Equal(
                requesterRecord.GetTag(TagConstants.LastThreadId),
                responderRecord.GetTag(TagConstants.LastThreadId));
            
            // Did Exchange Response
            requester.GetService<IEventAggregator>().GetEventByType<ServiceMessageProcessingEvent>()
                .Where(x => x.MessageType == MessageTypesHttps.DidExchange.Response)
                .Subscribe(x => slim.Release());
            
            var (response, newResponderRecord) = await didExchangeService.CreateResponseAsync(responder.Context, responderRecord);
            
            Assert.Equal(requestMessage.GetThreadId(), response.GetThreadId());
            Assert.Equal(invitation.Id, response.GetParentThreadId());
            
            await messsageService.SendAsync(responder.Context, response, newResponderRecord);
            
            await slim.WaitAsync(TimeSpan.FromSeconds(30));
            
            var newRequesterRecord = (await connectionService.ListAsync(requester.Context, SearchQuery.Equal(nameof(ConnectionRecord.TheirDid), newResponderRecord.MyDid))).First();
            
            Assert.Equal(ConnectionState.Connected, newResponderRecord.State);
            Assert.Equal(ConnectionState.Connected, newRequesterRecord.State);
            Assert.Equal(newRequesterRecord.TheirDid, newResponderRecord.MyDid);
            Assert.Equal(newResponderRecord.TheirDid, newRequesterRecord.MyDid);

            Assert.Equal(
                newRequesterRecord.GetTag(TagConstants.LastThreadId),
                newResponderRecord.GetTag(TagConstants.LastThreadId));
            
            // Did Exchange Complete
            responder.GetService<IEventAggregator>().GetEventByType<ServiceMessageProcessingEvent>()
                .Where(x => x.MessageType == MessageTypesHttps.DidExchange.Complete)
                .Subscribe(x => slim.Release());

            var (completeMessage, finalRequesterRecord) = await didExchangeService.CreateComplete(requester.Context, newRequesterRecord);
            
            Assert.Equal(invitation.Id, completeMessage.GetParentThreadId());
            Assert.Equal(response.GetThreadId(), completeMessage.GetThreadId());

            await messsageService.SendAsync(requester.Context, completeMessage, finalRequesterRecord);
            
            await slim.WaitAsync(TimeSpan.FromSeconds(30));
            
            var finalResponderRecord = (await connectionService.ListAsync(responder.Context, SearchQuery.Equal(nameof(ConnectionRecord.TheirDid), finalRequesterRecord.MyDid))).First();
            
            Assert.Equal(ConnectionState.Connected, finalResponderRecord.State);
            Assert.Equal(ConnectionState.Connected, finalRequesterRecord.State);
            Assert.Equal(finalRequesterRecord.TheirDid, finalResponderRecord.MyDid);
            Assert.Equal(finalResponderRecord.TheirDid, finalRequesterRecord.MyDid);
            
            Assert.Equal(
                finalRequesterRecord.GetTag(TagConstants.LastThreadId),
                finalResponderRecord.GetTag(TagConstants.LastThreadId));

            return (finalRequesterRecord, finalResponderRecord);
        }

        public static async Task<(ConnectionRecord inviteeConnection, ConnectionRecord inviterConnection)> EstablishConnectionWithReturnRoutingAsync(MockAgent invitee, MockAgent inviter, bool useDidKeyFormat = false)
        {
            var slim = new SemaphoreSlim(0, 1);

            var connectionService = inviter.GetService<IConnectionService>();
            var messageService = inviter.GetService<IMessageService>();

            // Hook into response message event of second runtime to release semaphore
            invitee.GetService<IEventAggregator>().GetEventByType<ServiceMessageProcessingEvent>()
                .Where(x => x.MessageType == MessageTypes.ConnectionResponse)
                .Subscribe(x => slim.Release());

            var (invitation, inviteeConnection) = await connectionService.CreateInvitationAsync(inviter.Context,
                new InviteConfiguration { AutoAcceptConnection = true, UseDidKeyFormat = useDidKeyFormat});

            var (request, inviterConnection) =
                await connectionService.CreateRequestAsync(invitee.Context, invitation);
            var response = await messageService.SendReceiveAsync<ConnectionResponseMessage>(invitee.Context, request, inviterConnection);

            Assert.NotNull(response);
            await connectionService.ProcessResponseAsync(invitee.Context, response, inviterConnection);

            await slim.WaitAsync(TimeSpan.FromSeconds(30));
            
            var ackMessage = await connectionService.CreateAcknowledgementMessageAsync(invitee.Context,
                inviterConnection.Id);
            await messageService.SendAsync(invitee.Context, ackMessage, inviterConnection);
            
            await slim.WaitAsync(TimeSpan.FromSeconds(30));

            var inviteeConnectionRecord = await connectionService.GetAsync(inviter.Context, inviteeConnection.Id);
            var inviterConnectionRecord = await connectionService.GetAsync(invitee.Context, inviterConnection.Id);

            Assert.Equal(ConnectionState.Connected, inviteeConnectionRecord.State);
            Assert.Equal(ConnectionState.Connected, inviterConnectionRecord.State);
            Assert.Equal(inviteeConnectionRecord.MyDid, inviterConnectionRecord.TheirDid);
            Assert.Equal(inviteeConnectionRecord.TheirDid, inviterConnectionRecord.MyDid);

            Assert.Equal(
                inviteeConnectionRecord.GetTag(TagConstants.LastThreadId),
                inviterConnectionRecord.GetTag(TagConstants.LastThreadId));

            return (inviteeConnectionRecord, inviterConnectionRecord);
        }

        public static async Task IssueCredentialAsync(MockAgent issuer, MockAgent holder, ConnectionRecord issuerConnection, ConnectionRecord holderConnection, List<CredentialPreviewAttribute> credentialAttributes)
        {
            var credentialService = issuer.GetService<ICredentialService>();
            var messageService = issuer.GetService<IMessageService>();
            var schemaService = issuer.GetService<ISchemaService>();
            var provisionService = issuer.GetService<IProvisioningService>();

            // Hook into message event
            var offerSlim = new SemaphoreSlim(0, 1);
            holder.GetService<IEventAggregator>().GetEventByType<ServiceMessageProcessingEvent>()
                .Where(x => x.MessageType == MessageTypes.IssueCredentialNames.OfferCredential)
                .Subscribe(x => offerSlim.Release());

            var issuerProv = await provisionService.GetProvisioningAsync(issuer.Context.Wallet);

            var (definitionId, _) = await Scenarios.CreateDummySchemaAndNonRevokableCredDef(issuer.Context, schemaService,
                issuerProv.IssuerDid, credentialAttributes.Select(_ => _.Name).ToArray());

            var (offer, issuerCredentialRecord) = await credentialService.CreateOfferAsync(
                agentContext: issuer.Context,
                config: new OfferConfiguration
                {
                    IssuerDid = issuerProv.IssuerDid,
                    CredentialDefinitionId = definitionId,
                    CredentialAttributeValues = credentialAttributes,
                },
                connectionId: issuerConnection.Id);
            await messageService.SendAsync(issuer.Context, offer, issuerConnection);

            await offerSlim.WaitAsync(TimeSpan.FromSeconds(30));

            var offers = await credentialService.ListOffersAsync(holder.Context);
            
            Assert.NotNull(offers);
            Assert.True(offers.Count > 0);

            // Hook into message event
            var requestSlim = new SemaphoreSlim(0, 1);
            issuer.GetService<IEventAggregator>().GetEventByType<ServiceMessageProcessingEvent>()
                .Where(x => x.MessageType == MessageTypes.IssueCredentialNames.RequestCredential)
                .Subscribe(x => requestSlim.Release());

            var (request, holderCredentialRecord) = await credentialService.CreateRequestAsync(holder.Context, offers[0].Id);

            Assert.NotNull(holderCredentialRecord.CredentialAttributesValues);
            
            Assert.True(holderCredentialRecord.CredentialAttributesValues.Count() == 2);

            await messageService.SendAsync(holder.Context, request, holderConnection);

            await requestSlim.WaitAsync(TimeSpan.FromSeconds(30));

            // Hook into message event
            var credentialSlim = new SemaphoreSlim(0, 1);
            holder.GetService<IEventAggregator>().GetEventByType<ServiceMessageProcessingEvent>()
                .Where(x => x.MessageType == MessageTypes.IssueCredentialNames.IssueCredential)
                .Subscribe(x => credentialSlim.Release());

            var (cred, _) = await credentialService.CreateCredentialAsync(
                agentContext: issuer.Context,
                credentialId: issuerCredentialRecord.Id);
            await messageService.SendAsync(issuer.Context, cred, issuerConnection);

            await credentialSlim.WaitAsync(TimeSpan.FromSeconds(30));

            var issuerCredRecord = await credentialService.GetAsync(issuer.Context, issuerCredentialRecord.Id);
            var holderCredRecord = await credentialService.GetAsync(holder.Context, holderCredentialRecord.Id);

            Assert.Equal(CredentialState.Issued, issuerCredRecord.State);
            Assert.Equal(CredentialState.Issued, holderCredRecord.State);

            Assert.Equal(
                issuerCredRecord.GetTag(TagConstants.LastThreadId),
                holderCredRecord.GetTag(TagConstants.LastThreadId));
            
            var ackSlim = new SemaphoreSlim(0, 1);
            holder.GetService<IEventAggregator>().GetEventByType<ServiceMessageProcessingEvent>()
                .Where(x => x.MessageType == MessageTypes.IssueCredentialNames.AcknowledgeCredential)
                .Subscribe(x => ackSlim.Release());
            
            var ackMessage =
                await credentialService.CreateAcknowledgementMessageAsync(holder.Context, holderCredentialRecord.Id);
            await messageService.SendAsync(holder.Context, ackMessage, holderConnection);

            await ackSlim.WaitAsync(TimeSpan.FromSeconds(30));
            
            Assert.Equal(ackMessage.Id, 
                issuerCredRecord.GetTag(TagConstants.LastThreadId));
        }
        
        public static async Task IssueCredentialConnectionlessAsync(MockAgent issuer, MockAgent holder, List<CredentialPreviewAttribute> credentialAttributes, bool useDidKeyFormat)
        {
            var credentialService = issuer.GetService<ICredentialService>();
            var schemaService = issuer.GetService<ISchemaService>();
            var provisionService = issuer.GetService<IProvisioningService>();

            var issuerProv = await provisionService.GetProvisioningAsync(issuer.Context.Wallet);

            var (definitionId, _) = await Scenarios.CreateDummySchemaAndNonRevokableCredDef(issuer.Context, schemaService,
                issuerProv.IssuerDid, credentialAttributes.Select(_ => _.Name).ToArray());

            (var offer, var issuerCredentialRecord) = await credentialService.CreateOfferAsync(
                agentContext: issuer.Context,
                config: new OfferConfiguration
                {
                    IssuerDid = issuerProv.IssuerDid,
                    CredentialDefinitionId = definitionId,
                    CredentialAttributeValues = credentialAttributes,
                    UseDidKeyFormat = useDidKeyFormat
                });

            var holderCredentialRecord = await credentialService.CreateCredentialAsync(holder.Context, offer);

            Assert.NotNull(holderCredentialRecord.CredentialAttributesValues);
            Assert.True(holderCredentialRecord.CredentialAttributesValues.Count() == 2);
            
            var issuerCredRecord = await credentialService.GetAsync(issuer.Context, issuerCredentialRecord.Id);
            var holderCredRecord = await credentialService.GetAsync(holder.Context, holderCredentialRecord.Id);

            Assert.Equal(CredentialState.Issued, issuerCredRecord.State);
            Assert.Equal(CredentialState.Issued, holderCredRecord.State);

            Assert.Equal(
                issuerCredRecord.GetTag(TagConstants.LastThreadId),
                holderCredRecord.GetTag(TagConstants.LastThreadId));
        }

        public static async Task ProofProtocolAsync(MockAgent requester, MockAgent holder,
            ConnectionRecord requesterConnection, ConnectionRecord holderConnection, ProofRequest proofRequest)
        {
            var proofService = requester.GetService<IProofService>();
            var messageService = requester.GetService<IMessageService>();

            // Hook into message event
            var requestSlim = new SemaphoreSlim(0, 1);
            holder.GetService<IEventAggregator>().GetEventByType<ServiceMessageProcessingEvent>()
                .Where(x => x.MessageType == MessageTypes.PresentProofNames.RequestPresentation)
                .Subscribe(x => requestSlim.Release());

            var (requestMsg, requesterRecord) = await proofService.CreateRequestAsync(requester.Context, proofRequest, requesterConnection.Id);
            await messageService.SendAsync(requester.Context, requestMsg, requesterConnection);

            await requestSlim.WaitAsync(TimeSpan.FromSeconds(30));

            var holderRequests = await proofService.ListRequestedAsync(holder.Context);

            Assert.NotNull(holderRequests);
            Assert.True(holderRequests.Count > 0);

            // Hook into message event
            var proofSlim = new SemaphoreSlim(0, 1);
            requester.GetService<IEventAggregator>().GetEventByType<ServiceMessageProcessingEvent>()
                .Where(x => x.MessageType == MessageTypes.PresentProofNames.Presentation)
                .Subscribe(x => proofSlim.Release());

            var record = holderRequests.FirstOrDefault();
            var request = JsonConvert.DeserializeObject<ProofRequest>(record.RequestJson);

            var requestedCredentials =
                await ProofServiceUtils.GetAutoRequestedCredentialsForProofCredentials(holder.Context, proofService,
                    request);

            var (proofMsg, holderRecord) = await proofService.CreatePresentationAsync(holder.Context, record.Id, requestedCredentials);
            await messageService.SendAsync(holder.Context, proofMsg, holderConnection);

            await proofSlim.WaitAsync(TimeSpan.FromSeconds(30));

            var requesterProofRecord = await proofService.GetAsync(requester.Context, requesterRecord.Id);
            var holderProofRecord = await proofService.GetAsync(holder.Context, holderRecord.Id);

            Assert.True(requesterProofRecord.State == ProofState.Accepted);
            Assert.True(holderProofRecord.State == ProofState.Accepted);

            var isProofValid = await proofService.VerifyProofAsync(requester.Context, requesterProofRecord.Id);
            Assert.True(isProofValid);
            
            var ackSlim = new SemaphoreSlim(0, 1);
            holder.GetService<IEventAggregator>().GetEventByType<ServiceMessageProcessingEvent>()
                .Where(x => x.MessageType == MessageTypes.PresentProofNames.AcknowledgePresentation)
                .Subscribe(x => ackSlim.Release());
            
            var acknowledgeMessage = await proofService.CreateAcknowledgeMessageAsync(requester.Context, requesterProofRecord.Id);
            await messageService.SendAsync(requester.Context, acknowledgeMessage, requesterConnection);

            await ackSlim.WaitAsync(TimeSpan.FromSeconds(30));
        }
        
        public static async Task ProofProtocolConnectionlessAsync(MockAgent requestor, MockAgent holder, ProofRequest proofRequest, bool useDidKeyFormat)
        {
            var proofService = requestor.GetService<IProofService>();

            var (requestMsg, requestorRecord) = await proofService.CreateRequestAsync(requestor.Context, proofRequest, useDidKeyFormat: useDidKeyFormat);

            var requestAttachment = requestMsg.Requests.FirstOrDefault(x => x.Id == "libindy-request-presentation-0")
                                    ?? throw new ArgumentException("Presentation request attachment not found.");
            
            var requestJson = requestAttachment.Data.Base64.GetBytesFromBase64().GetUTF8String();
            var request = JsonConvert.DeserializeObject<ProofRequest>(requestJson);

            var requestedCredentials =
                await ProofServiceUtils.GetAutoRequestedCredentialsForProofCredentials(holder.Context, proofService,
                    request);

            var holderRecord = await proofService.CreatePresentationAsync(holder.Context, requestMsg, requestedCredentials);

            var requestorProofRecord = await proofService.GetAsync(requestor.Context, requestorRecord.Id);
            var holderProofRecord = await proofService.GetAsync(holder.Context, holderRecord.Id);

            Assert.True(requestorProofRecord.State == ProofState.Accepted);
            Assert.True(holderProofRecord.State == ProofState.Accepted);

            var isProofValid = await proofService.VerifyProofAsync(requestor.Context, requestorProofRecord.Id);

            Assert.True(isProofValid);
        }

        public static async Task<DiscoveryDiscloseMessage> DiscoveryProtocolWithReturnRoutingAsync(MockAgent requestor, MockAgent holder, ConnectionRecord requestorConnection, ConnectionRecord holderConnection)
        {
            var discoveryService = requestor.GetService<IDiscoveryService>();
            var messageService = requestor.GetService<IMessageService>();

            //Ask for all protocols
            var msg = discoveryService.CreateQuery(requestor.Context, "*");
            var rsp = await messageService.SendReceiveAsync(requestor.Context, msg, requestorConnection);

            Assert.NotNull(rsp);

            if (rsp is UnpackedMessageContext messageContext)
            {
                var discoveryMsg = messageContext.GetMessage<DiscoveryDiscloseMessage>();

                Assert.NotNull(discoveryMsg);

                return discoveryMsg;
            }
            throw new InvalidOperationException("The response was not of the expected type");
        }

        public static string CreateBasicMessageWithLegacyType(MockAgent LegacyMessageAgent)
        {
            var basicMessage = new BasicMessage(LegacyMessageAgent.Context.UseMessageTypesHttps);

            Assert.Equal(MessageTypes.BasicMessageType, basicMessage.Type);

            return basicMessage.Type;
        }

        public static string CreateBasicMessageWithHttpsType(MockAgent HttpsMessageAgent)
        {

            var basicMessage = new BasicMessage(HttpsMessageAgent.Context.UseMessageTypesHttps);

            Assert.Equal(MessageTypesHttps.BasicMessageType, basicMessage.Type);

            return basicMessage.Type;
        }
    }
}
