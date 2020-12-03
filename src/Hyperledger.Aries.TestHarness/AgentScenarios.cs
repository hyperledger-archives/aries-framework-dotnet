using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hyperledger.Aries;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Features.BasicMessage;
using Hyperledger.Aries.Features.DidExchange;
using Hyperledger.Aries.Features.Discovery;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Features.PresentProof;
using Hyperledger.Aries.Models.Events;
using Hyperledger.Aries.Utils;
using Hyperledger.TestHarness.Mock;
using Hyperledger.TestHarness.Utils;
using Newtonsoft.Json;
using Xunit;

namespace Hyperledger.TestHarness
{
    public static class AgentScenarios
    {
        public static async Task<(ConnectionRecord inviteeConnection,ConnectionRecord inviterConnection)> EstablishConnectionAsync(MockAgent invitee, MockAgent inviter)
        {
            var slim = new SemaphoreSlim(0, 1);
            
            var connectionService = invitee.GetService<IConnectionService>();
            var messsageService = invitee.GetService<IMessageService>();

            // Hook into response message event of second runtime to release semaphore
            inviter.GetService<IEventAggregator>().GetEventByType<ServiceMessageProcessingEvent>()
                .Where(x => x.MessageType == MessageTypes.ConnectionResponse)
                .Subscribe(x => slim.Release());

            (var invitation, var inviterConnection) = await connectionService.CreateInvitationAsync(invitee.Context,
                new InviteConfiguration { AutoAcceptConnection = true });

            (var request, var inviteeConnection) =
                await connectionService.CreateRequestAsync(inviter.Context, invitation);
            await messsageService.SendAsync(inviter.Context, request, inviteeConnection);

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

        public static async Task<(ConnectionRecord inviteeConnection, ConnectionRecord inviterConnection)> EstablishConnectionWithReturnRoutingAsync(MockAgent invitee, MockAgent inviter)
        {
            var slim = new SemaphoreSlim(0, 1);

            var connectionService = invitee.GetService<IConnectionService>();
            var messsageService = invitee.GetService<IMessageService>();

            // Hook into response message event of second runtime to release semaphore
            inviter.GetService<IEventAggregator>().GetEventByType<ServiceMessageProcessingEvent>()
                .Where(x => x.MessageType == MessageTypes.ConnectionResponse)
                .Subscribe(x => slim.Release());

            (var invitation, var inviterConnection) = await connectionService.CreateInvitationAsync(invitee.Context,
                new InviteConfiguration { AutoAcceptConnection = true });

            (var request, var inviteeConnection) =
                await connectionService.CreateRequestAsync(inviter.Context, invitation);
            var response = await messsageService.SendReceiveAsync<ConnectionResponseMessage>(inviter.Context, request, inviteeConnection);

            Assert.NotNull(response);
            await connectionService.ProcessResponseAsync(inviter.Context, response, inviteeConnection);

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

        public static async Task IssueCredentialAsync(MockAgent issuer, MockAgent holder, ConnectionRecord issuerConnection, ConnectionRecord holderConnection, List<CredentialPreviewAttribute> credentialAttributes)
        {
            var credentialService = issuer.GetService<ICredentialService>();
            var messsageService = issuer.GetService<IMessageService>();
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

            (var offer, var issuerCredentialRecord) = await credentialService.CreateOfferAsync(
                agentContext: issuer.Context,
                config: new OfferConfiguration
                {
                    IssuerDid = issuerProv.IssuerDid,
                    CredentialDefinitionId = definitionId,
                    CredentialAttributeValues = credentialAttributes,
                },
                connectionId: issuerConnection.Id);
            await messsageService.SendAsync(issuer.Context, offer, issuerConnection);

            await offerSlim.WaitAsync(TimeSpan.FromSeconds(30));

            var offers = await credentialService.ListOffersAsync(holder.Context);
            
            Assert.NotNull(offers);
            Assert.True(offers.Count > 0);

            // Hook into message event
            var requestSlim = new SemaphoreSlim(0, 1);
            issuer.GetService<IEventAggregator>().GetEventByType<ServiceMessageProcessingEvent>()
                .Where(x => x.MessageType == MessageTypes.IssueCredentialNames.RequestCredential)
                .Subscribe(x => requestSlim.Release());

            (var request, var holderCredentialRecord) = await credentialService.CreateRequestAsync(holder.Context, offers[0].Id);

            Assert.NotNull(holderCredentialRecord.CredentialAttributesValues);
            
            Assert.True(holderCredentialRecord.CredentialAttributesValues.Count() == 2);

            await messsageService.SendAsync(holder.Context, request, holderConnection);

            await requestSlim.WaitAsync(TimeSpan.FromSeconds(30));

            // Hook into message event
            var credentialSlim = new SemaphoreSlim(0, 1);
            holder.GetService<IEventAggregator>().GetEventByType<ServiceMessageProcessingEvent>()
                .Where(x => x.MessageType == MessageTypes.IssueCredentialNames.IssueCredential)
                .Subscribe(x => credentialSlim.Release());

            (var cred, _) = await credentialService.CreateCredentialAsync(
                agentContext: issuer.Context,
                credentialId: issuerCredentialRecord.Id);
            await messsageService.SendAsync(issuer.Context, cred, issuerConnection);

            await credentialSlim.WaitAsync(TimeSpan.FromSeconds(30));

            var issuerCredRecord = await credentialService.GetAsync(issuer.Context, issuerCredentialRecord.Id);
            var holderCredRecord = await credentialService.GetAsync(holder.Context, holderCredentialRecord.Id);

            Assert.Equal(CredentialState.Issued, issuerCredRecord.State);
            Assert.Equal(CredentialState.Issued, holderCredRecord.State);

            Assert.Equal(
                issuerCredRecord.GetTag(TagConstants.LastThreadId),
                holderCredRecord.GetTag(TagConstants.LastThreadId));
        }

        public static async Task ProofProtocolAsync(MockAgent requestor, MockAgent holder,
            ConnectionRecord requestorConnection, ConnectionRecord holderConnection, ProofRequest proofRequest)
        {
            var proofService = requestor.GetService<IProofService>();
            var messageService = requestor.GetService<IMessageService>();

            // Hook into message event
            var requestSlim = new SemaphoreSlim(0, 1);
            holder.GetService<IEventAggregator>().GetEventByType<ServiceMessageProcessingEvent>()
                .Where(x => x.MessageType == MessageTypes.PresentProofNames.RequestPresentation)
                .Subscribe(x => requestSlim.Release());

            var (requestMsg, requestorRecord) = await proofService.CreateRequestAsync(requestor.Context, proofRequest, requestorConnection.Id);
            await messageService.SendAsync(requestor.Context, requestMsg, requestorConnection);

            await requestSlim.WaitAsync(TimeSpan.FromSeconds(30));

            var holderRequests = await proofService.ListRequestedAsync(holder.Context);

            Assert.NotNull(holderRequests);
            Assert.True(holderRequests.Count > 0);

            // Hook into message event
            var proofSlim = new SemaphoreSlim(0, 1);
            requestor.GetService<IEventAggregator>().GetEventByType<ServiceMessageProcessingEvent>()
                .Where(x => x.MessageType == MessageTypes.PresentProofNames.Presentation)
                .Subscribe(x => requestSlim.Release());

            var record = holderRequests.FirstOrDefault();
            var request = JsonConvert.DeserializeObject<ProofRequest>(record.RequestJson);

            var requestedCredentials =
                await ProofServiceUtils.GetAutoRequestedCredentialsForProofCredentials(holder.Context, proofService,
                    request);

            var (proofMsg, holderRecord) = await proofService.CreatePresentationAsync(holder.Context, record.Id, requestedCredentials);
            await messageService.SendAsync(holder.Context, proofMsg, holderConnection);

            await proofSlim.WaitAsync(TimeSpan.FromSeconds(30));

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
