using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.Handshakes.Common;
using Hyperledger.Aries.Features.Handshakes.Connection;
using Hyperledger.Aries.Features.Handshakes.Connection.Models;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Features.PresentProof;
using Hyperledger.Indy.AnonCredsApi;
using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.PoolApi;
using Hyperledger.TestHarness;
using Hyperledger.TestHarness.Utils;
using Newtonsoft.Json;
using Xunit;

namespace Hyperledger.Aries.TestHarness
{
    public static class Scenarios
    {
        public static async Task<(ConnectionRecord firstParty, ConnectionRecord secondParty)> EstablishConnectionAsync(
            IConnectionService connectionService,
            IProducerConsumerCollection<AgentMessage> messages,
            IAgentContext inviterContext,
            IAgentContext inviteeContext,
            ConnectionInvitationMessage inviteMessage = null,
            string inviteeConnectionId = null,
            bool useDidKeyFormat = false)
        {
            // Create invitation by the issuer
            inviteeConnectionId ??= Guid.NewGuid().ToString();

            var inviteConfig = new InviteConfiguration
            {
                ConnectionId = inviteeConnectionId,
                MyAlias = new ConnectionAlias
                {
                    Name = "Issuer",
                    ImageUrl = "www.issuerdomain.com/profilephoto"
                },
                TheirAlias = new ConnectionAlias
                {
                    Name = "Holder",
                    ImageUrl = "www.holderdomain.com/profilephoto"
                },
                UseDidKeyFormat = useDidKeyFormat
            };

            if (inviteMessage == null)
                (inviteMessage, _) = await connectionService.CreateInvitationAsync(inviterContext, inviteConfig);

            var recordOnInviterSide = await connectionService.GetAsync(inviterContext, inviteConfig.ConnectionId);
            Assert.Equal(ConnectionState.Invited, recordOnInviterSide.State);

            // Holder accepts invitation and sends a message request
            var (request, recordOnInviteeSide) = await connectionService.CreateRequestAsync(inviteeContext, inviteMessage);

            messages.TryAdd(request);

            Assert.Equal(ConnectionState.Negotiating, recordOnInviteeSide.State);

            // Issuer processes incoming message
            var issuerMessage = messages.OfType<ConnectionRequestMessage>().FirstOrDefault();
            Assert.NotNull(issuerMessage);

            // Issuer processes the connection request by storing it and accepting it if auto connection flow is enabled
            inviteeConnectionId = await connectionService.ProcessRequestAsync(inviterContext, issuerMessage, recordOnInviterSide);

            recordOnInviterSide = await connectionService.GetAsync(inviterContext, inviteeConnectionId);
            Assert.Equal(ConnectionState.Negotiating, recordOnInviterSide.State);

            // Issuer accepts the connection request
            var (response, _) = await connectionService.CreateResponseAsync(inviterContext, inviteeConnectionId);
            messages.TryAdd(response);

            recordOnInviterSide = await connectionService.GetAsync(inviterContext, inviteeConnectionId);
            Assert.Equal(ConnectionState.Connected, recordOnInviterSide.State);

            // Holder processes incoming message
            var holderMessage = messages.OfType<ConnectionResponseMessage>().FirstOrDefault();
            Assert.NotNull(holderMessage);

            // Holder processes the response message by accepting it
            await connectionService.ProcessResponseAsync(inviteeContext, holderMessage, recordOnInviteeSide);
            
            // Retrieve updated connection state for both issuer and holder
            recordOnInviterSide = await connectionService.GetAsync(inviterContext, recordOnInviterSide.Id);
            recordOnInviteeSide = await connectionService.GetAsync(inviteeContext, recordOnInviteeSide.Id);

            return (recordOnInviterSide, recordOnInviteeSide);
        }

        public static async Task<(CredentialRecord issuerCredential, CredentialRecord holderCredential)> IssueCredentialAsync(
            ISchemaService schemaService, ICredentialService credentialService,
            IProducerConsumerCollection<AgentMessage> messages,
            ConnectionRecord issuerConnection, ConnectionRecord holderConnection,
            IAgentContext issuerContext,
            IAgentContext holderContext,
            Pool pool, string proverMasterSecretId, bool revocable, List<CredentialPreviewAttribute> credentialAttributes, OfferConfiguration offerConfiguration = null)
        {
            // Create an issuer DID/VK. Can also be created during provisioning
            var issuer = await Did.CreateAndStoreMyDidAsync(issuerContext.Wallet,
                new { seed = TestConstants.StewardSeed }.ToJson());

            // Create a schema and credential definition for this issuer
            var (definitionId, _) = await CreateDummySchemaAndNonRevokableCredDef(issuerContext, schemaService,
                issuer.Did, credentialAttributes.Select(_ => _.Name).ToArray());

            var offerConfig = offerConfiguration ?? new OfferConfiguration
            {
                IssuerDid = issuer.Did,
                CredentialDefinitionId = definitionId,
                CredentialAttributeValues = credentialAttributes
            };

            // Send an offer to the holder using the established connection channel
            var (offerMessage, _) = await credentialService.CreateOfferAsync(
                agentContext: issuerContext,
                config: offerConfig,
                connectionId: issuerConnection.Id);
            messages.TryAdd(offerMessage);

            // Holder retrieves message from their cloud agent
            var credentialOffer = FindContentMessage<CredentialOfferMessage>(messages);

            // Holder processes the credential offer by storing it
            var holderCredentialId =
                await credentialService.ProcessOfferAsync(holderContext, credentialOffer, holderConnection);

            // Holder creates master secret. Will also be created during wallet agent provisioning
            await AnonCreds.ProverCreateMasterSecretAsync(holderContext.Wallet, proverMasterSecretId);

            // Holder accepts the credential offer and sends a credential request
            var (request, _) = await credentialService.CreateRequestAsync(holderContext, holderCredentialId);
            messages.TryAdd(request);

            // Issuer retrieves credential request from cloud agent
            var credentialRequest = FindContentMessage<CredentialRequestMessage>(messages);
            Assert.NotNull(credentialRequest);

            // Issuer processes the credential request by storing it
            var issuerCredentialId =
                await credentialService.ProcessCredentialRequestAsync(issuerContext, credentialRequest, issuerConnection);

            // Issuer accepts the credential requests and issues a credential
            var (credentialMessage, _) = await credentialService.CreateCredentialAsync(
                agentContext: issuerContext,
                credentialId: issuerCredentialId,
                values: credentialAttributes);
            messages.TryAdd(credentialMessage);

            // Holder retrieves the credential from their cloud agent
            var credential = FindContentMessage<CredentialIssueMessage>(messages);
            Assert.NotNull(credential);

            // Holder processes the credential by storing it in their wallet
            await credentialService.ProcessCredentialAsync(holderContext, credential, holderConnection);

            // Verify states of both credential records are set to 'Issued'
            var issuerCredential = await credentialService.GetAsync(issuerContext, issuerCredentialId);
            var holderCredential = await credentialService.GetAsync(holderContext, holderCredentialId);

            return (issuerCredential, holderCredential);
        }

        public static async Task<(ProofRecord holderProofRecord, ProofRecord requestorProofRecord)> ProposerInitiatedProofProtocolAsync(
            IProofService proofService,
            IProducerConsumerCollection<AgentMessage> messages,
            ConnectionRecord holderConnection, ConnectionRecord requestorConnection,
            IAgentContext holderContext, IAgentContext requestorContext,
            ProofProposal proofProposalObject)
        {
            // Holder sends a proof proposal
            var (holderProposalMessage, holderProofProposalRecord) = await proofService.CreateProposalAsync(holderContext, proofProposalObject, holderConnection.Id);
            messages.TryAdd(holderProposalMessage);
            Assert.True(holderProofProposalRecord.State == ProofState.Proposed);
            // Requestor accepts the proposal and builds a proofRequest
            var requestorProposalMessage = FindContentMessage<ProposePresentationMessage>(messages);
            Assert.NotNull(requestorProposalMessage);

            //Requestor stores the proof proposal
            var requestorProofProposalRecord = await proofService.ProcessProposalAsync(requestorContext, requestorProposalMessage, requestorConnection);
            Assert.Equal(ProofState.Proposed, requestorProofProposalRecord.State);
            var proposal = requestorProofProposalRecord.ProposalJson.ToObject<ProofProposal>();
            Assert.Equal("Hello, World", proposal.Comment);
            Assert.NotNull(proposal.ProposedAttributes);
            Console.WriteLine(requestorProofProposalRecord.ProposalJson);
            // Requestor sends a proof request
            var (requestorRequestMessage, requestorProofRequestRecord) = await proofService.CreateRequestFromProposalAsync(
                requestorContext,
                new ProofRequestParameters
                {
                    Name = "Test",
                    Version = "1.0"
                },
                requestorProofProposalRecord.Id, requestorConnection.Id);
            messages.TryAdd(requestorRequestMessage);
            Assert.Equal(ProofState.Requested, requestorProofRequestRecord.State);

            return await ProofProtocolAsync(proofService, messages, holderConnection, requestorConnection, holderContext, requestorContext, requestorProofRequestRecord);
        }

        public static async Task<(ProofRecord holderProofRecord, ProofRecord requestorProofRecord)> RequestorInitiatedProofProtocolAsync(
            IProofService proofService,
            IProducerConsumerCollection<AgentMessage> messages,
            ConnectionRecord holderConnection, ConnectionRecord requestorConnection,
            IAgentContext holderContext,
            IAgentContext requestorContext, ProofRequest proofRequestObject)
        {

            //Requestor sends a proof request
            var (message, requestorProofRecord) = await proofService.CreateRequestAsync(requestorContext, proofRequestObject, requestorConnection.Id);
            messages.TryAdd(message);

            return await ProofProtocolAsync(proofService, messages, holderConnection, requestorConnection, holderContext, requestorContext, requestorProofRecord);
        }

        public static async Task<(ProofRecord holderProofRecord, ProofRecord RequestorProofRecord)> ProofProtocolAsync(
            IProofService proofService,
            IProducerConsumerCollection<AgentMessage> messages,
            ConnectionRecord holderConnection, ConnectionRecord requestorConnection,
            IAgentContext holderContext,
            IAgentContext requestorContext, ProofRecord requestorProofRecord)
        {

            // Holder accepts the proof requests and builds a proof
            var holderRequestPresentationMessage = FindContentMessage<RequestPresentationMessage>(messages);
            Assert.NotNull(holderRequestPresentationMessage);

            //Holder stores the proof request if they haven't created it already
            var holderProofRecord = await proofService.ProcessRequestAsync(holderContext, holderRequestPresentationMessage, holderConnection);
            Assert.Equal(ProofState.Requested, holderProofRecord.State);
            Console.WriteLine(holderProofRecord.RequestJson);


            var holderProofRequest = JsonConvert.DeserializeObject<ProofRequest>(holderProofRecord.RequestJson);

            // Auto satisfy the proof with which ever credentials in the wallet are capable
            var requestedCredentials =
                await ProofServiceUtils.GetAutoRequestedCredentialsForProofCredentials(holderContext, proofService,
                    holderProofRequest);

            //Holder accepts the proof request and sends a proof
            (var proofMessage, _) = await proofService.CreatePresentationAsync(
                holderContext,
                holderProofRecord.Id,
                requestedCredentials);
            messages.TryAdd(proofMessage);

            //Requestor retrives proof message from their cloud agent
            var proof = FindContentMessage<PresentationMessage>(messages);
            Assert.NotNull(proof);

            //Requestor stores proof
            requestorProofRecord = await proofService.ProcessPresentationAsync(requestorContext, proof);
            //Requestor verifies proof
            var requestorVerifyResult = await proofService.VerifyProofAsync(requestorContext, requestorProofRecord.Id);

            //Verify the proof is valid
            Assert.True(requestorVerifyResult);

            var requestorProofRecordResult = await proofService.GetAsync(requestorContext, requestorProofRecord.Id);
            var holderProofRecordResult = await proofService.GetAsync(holderContext, holderProofRecord.Id);

            return (holderProofRecordResult, requestorProofRecordResult);
        }

        public static async Task<(string, string)> CreateDummySchemaAndNonRevokableCredDef(IAgentContext context, ISchemaService schemaService, string issuerDid, string[] attributeValues)
        {
            // Create a schema and credential definition for this issuer
            var schemaId = await schemaService.CreateSchemaAsync(context, issuerDid,
                $"Test-Schema-{Guid.NewGuid()}", "1.0", attributeValues);
            var credentialDefinitionConfiguration = new CredentialDefinitionConfiguration
            {
                SchemaId = schemaId,
                EnableRevocation = false,
                IssuerDid = issuerDid,
                Tag = "Tag",
            };
            return
            (
                await schemaService.CreateCredentialDefinitionAsync(context, credentialDefinitionConfiguration),
                schemaId
            );
        }

        private static T FindContentMessage<T>(IEnumerable<AgentMessage> collection)
            where T : AgentMessage
            => collection.OfType<T>().Single();
    }
}
