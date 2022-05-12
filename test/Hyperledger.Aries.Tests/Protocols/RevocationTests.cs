using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Features.PresentProof;
using Hyperledger.Aries.Models.Events;
using Hyperledger.Indy.AnonCredsApi;
using Hyperledger.TestHarness;
using Hyperledger.TestHarness.Mock;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Hyperledger.Aries.Tests.Protocols
{
    public class RevocationTestsFixture : TestSingleWallet
    {
        public InProcAgent.PairedAgents PairedAgents;
        
        public IAgentContext IssuerAgentContext;
        public IAgentContext HolderAgentContext;
        
        public ICredentialService IssuerCredentialService;
        public ICredentialService HolderCredentialService;

        public IEventAggregator EventAggregator;

        public IProofService IssuerProofService;
        public IProofService HolderProofService;

        public IMessageService IssuerMessageService;
        public IMessageService HolderMessageService;

        public ProvisioningRecord IssuerConfiguration;

        public string RevocableCredentialDefinitionId;
        public string NonRevocableCredentialDefinitionId;
        
        private string _credentialSchemaId;

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            
            PairedAgents = await InProcAgent.CreatePairedAsync(true);

            IssuerAgentContext = PairedAgents.Agent1.Context;
            HolderAgentContext = PairedAgents.Agent2.Context;
            
            EventAggregator = PairedAgents.Agent2.Provider.GetService<IEventAggregator>();
            
            IssuerCredentialService = PairedAgents.Agent1.Provider.GetService<ICredentialService>();
            HolderCredentialService = PairedAgents.Agent2.Provider.GetService<ICredentialService>();
            
            IssuerProofService = PairedAgents.Agent1.Provider.GetService<IProofService>();
            HolderProofService = PairedAgents.Agent2.Provider.GetService<IProofService>();
            
            IssuerMessageService = PairedAgents.Agent1.Provider.GetRequiredService<IMessageService>();
            HolderMessageService = PairedAgents.Agent2.Provider.GetService<IMessageService>();
            
            IssuerConfiguration = await PairedAgents.Agent1.Provider.GetRequiredService<IProvisioningService>()
                .GetProvisioningAsync(IssuerAgentContext.Wallet);
            await PromoteTrustAnchor(IssuerConfiguration.IssuerDid, IssuerConfiguration.IssuerVerkey);
            
            _credentialSchemaId = await PairedAgents.Agent1.Provider.GetRequiredService<ISchemaService>()
                .CreateSchemaAsync(
                    context: IssuerAgentContext,
                    issuerDid: IssuerConfiguration.IssuerDid,
                    name: $"test-schema-{Guid.NewGuid()}",
                    version: "1.0",
                    attributeNames: new[] { "name", "age" });
            
            RevocableCredentialDefinitionId = await PairedAgents.Agent1.Provider.GetRequiredService<ISchemaService>()
                .CreateCredentialDefinitionAsync(
                    context: IssuerAgentContext,
                    new CredentialDefinitionConfiguration
                    {
                        SchemaId = _credentialSchemaId,
                        EnableRevocation = true,
                        RevocationRegistryBaseUri = "http://localhost",
                        Tag = "revoc"
                    });
            
            NonRevocableCredentialDefinitionId = await PairedAgents.Agent1.Provider.GetRequiredService<ISchemaService>()
                .CreateCredentialDefinitionAsync(
                    context: IssuerAgentContext,
                    new CredentialDefinitionConfiguration
                    {
                        SchemaId = _credentialSchemaId,
                        EnableRevocation = false,
                        RevocationRegistryBaseUri = "http://localhost",
                        Tag = "norevoc"
                    });
        }

        public override async Task DisposeAsync()
        {
            await base.DisposeAsync();
            await PairedAgents.Agent1.DisposeAsync();
            await PairedAgents.Agent2.DisposeAsync();
        }
    }
    
    public class RevocationTests : IClassFixture<RevocationTestsFixture>, IAsyncLifetime
    {
        private readonly RevocationTestsFixture _fixture;
        private readonly uint _now = (uint) DateTimeOffset.Now.ToUnixTimeSeconds();

        public RevocationTests(RevocationTestsFixture data)
        {
            _fixture = data;
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            foreach (var credentialRecord in await _fixture.HolderCredentialService.ListAsync(_fixture.HolderAgentContext))
                await _fixture.HolderCredentialService.DeleteCredentialAsync(
                    _fixture.HolderAgentContext, credentialRecord.Id);
            
            foreach (var credentialRecord in await _fixture.IssuerCredentialService.ListAsync(_fixture.IssuerAgentContext))
                await _fixture.IssuerCredentialService.DeleteCredentialAsync(
                    _fixture.IssuerAgentContext, credentialRecord.Id);
        }

        [Fact(DisplayName = "Test credential revocation")]
        public async Task CanRevokeCredential()
        {
            var receivedRevocationNotificationMessage = false;
            var receivedRevocationNotificationAckMessage = false;

            _fixture.EventAggregator.GetEventByType<ServiceMessageProcessingEvent>()
                .Where(x => x.MessageType == MessageTypesHttps.RevocationNotification)
                .Subscribe(_ => receivedRevocationNotificationMessage = true);
            
            _fixture.EventAggregator.GetEventByType<ServiceMessageProcessingEvent>()
                .Where(x => x.MessageType == MessageTypesHttps.RevocationNotification)
                .Subscribe(_ => receivedRevocationNotificationAckMessage = true);
            
            
            var (offer, record) = await _fixture.IssuerCredentialService
                .CreateOfferAsync(_fixture.IssuerAgentContext, new OfferConfiguration
                {
                    CredentialDefinitionId = _fixture.RevocableCredentialDefinitionId,
                    IssuerDid = _fixture.IssuerConfiguration.IssuerDid,
                    CredentialAttributeValues = new[]
                    {
                        new CredentialPreviewAttribute("name", "random"),
                        new CredentialPreviewAttribute("age", "22")
                    }
                });
            await _fixture.IssuerMessageService.SendAsync(_fixture.IssuerAgentContext, offer, _fixture.PairedAgents.Connection1);
            
            var credentialRecordOnHolderSide = (await _fixture.HolderCredentialService.ListAsync(_fixture.HolderAgentContext))
                .First(credentialRecord => credentialRecord.State == CredentialState.Offered);
            var (request, _) = await _fixture.HolderCredentialService.CreateRequestAsync(_fixture.HolderAgentContext, credentialRecordOnHolderSide.Id);
            await _fixture.HolderMessageService.SendAsync(_fixture.HolderAgentContext, request, _fixture.PairedAgents.Connection2);

            var credentialRecordOnIssuerSide = (await _fixture.IssuerCredentialService.ListRequestsAsync(
                _fixture.IssuerAgentContext)).First();
            var (issue, _) = await _fixture.IssuerCredentialService.CreateCredentialAsync(_fixture.IssuerAgentContext, credentialRecordOnIssuerSide.Id);
            await _fixture.IssuerMessageService.SendAsync(_fixture.IssuerAgentContext, issue, _fixture.PairedAgents.Connection1);
            
            credentialRecordOnHolderSide =
                await _fixture.HolderCredentialService.GetAsync(_fixture.HolderAgentContext,
                    credentialRecordOnHolderSide.Id);
            credentialRecordOnIssuerSide =
                await _fixture.IssuerCredentialService.GetAsync(_fixture.IssuerAgentContext,
                    credentialRecordOnIssuerSide.Id);
            
            Assert.Equal(CredentialState.Issued, credentialRecordOnHolderSide.State);
            Assert.Equal(CredentialState.Issued, credentialRecordOnIssuerSide.State);

            await _fixture.IssuerCredentialService.RevokeCredentialAsync(
                _fixture.IssuerAgentContext, credentialRecordOnIssuerSide.Id, true);

            Assert.True(
                await _fixture.HolderProofService.IsRevokedAsync(
                    _fixture.HolderAgentContext,
                    credentialRecordOnHolderSide.Id));
            Assert.True(
                await _fixture.IssuerProofService.IsRevokedAsync(
                    _fixture.IssuerAgentContext,
                    credentialRecordOnIssuerSide.Id));
            
            Assert.True(receivedRevocationNotificationMessage);
            Assert.True(receivedRevocationNotificationAckMessage);
        }

        [Fact(DisplayName = "Test verification without revocation")]
        public async Task CanVerifyWithoutRevocation()
        {
            var (offer, record) = await _fixture.IssuerCredentialService
                .CreateOfferAsync(_fixture.IssuerAgentContext, new OfferConfiguration
                {
                    CredentialDefinitionId = _fixture.NonRevocableCredentialDefinitionId,
                    IssuerDid = _fixture.IssuerConfiguration.IssuerDid,
                    CredentialAttributeValues = new[]
                    {
                        new CredentialPreviewAttribute("name", "random"),
                        new CredentialPreviewAttribute("age", "22")
                    }
                });
            await _fixture.IssuerMessageService
                .SendAsync(_fixture.IssuerAgentContext, offer, _fixture.PairedAgents.Connection1);
            
            var credentialRecordOnHolderSide = (await _fixture.HolderCredentialService.ListAsync(_fixture.HolderAgentContext))
                .First(credentialRecord => credentialRecord.State == CredentialState.Offered);
            var (request, _) = await _fixture.HolderCredentialService.CreateRequestAsync(_fixture.HolderAgentContext, credentialRecordOnHolderSide.Id);
            await _fixture.HolderMessageService.SendAsync(_fixture.HolderAgentContext, request, _fixture.PairedAgents.Connection2);

            var credentialRecordOnIssuerSide = (await _fixture.IssuerCredentialService.ListRequestsAsync(
                _fixture.IssuerAgentContext)).First();
            var (issue, _) = await _fixture.IssuerCredentialService.CreateCredentialAsync(_fixture.IssuerAgentContext, credentialRecordOnIssuerSide.Id);
            await _fixture.IssuerMessageService.SendAsync(_fixture.IssuerAgentContext, issue, _fixture.PairedAgents.Connection1);
            
            credentialRecordOnHolderSide =
                await _fixture.HolderCredentialService.GetAsync(_fixture.HolderAgentContext,
                    credentialRecordOnHolderSide.Id);
            credentialRecordOnIssuerSide =
                await _fixture.IssuerCredentialService.GetAsync(_fixture.IssuerAgentContext,
                    credentialRecordOnIssuerSide.Id);
            
            Assert.Equal(CredentialState.Issued, credentialRecordOnHolderSide.State);
            Assert.Equal(CredentialState.Issued, credentialRecordOnIssuerSide.State);
            
            var (requestPresentationMessage, proofRecordIssuer) = await _fixture.IssuerProofService
                .CreateRequestAsync(_fixture.IssuerAgentContext, new ProofRequest
                {
                    Name = "Test Verification",
                    Version = "1.0",
                    Nonce = await AnonCreds.GenerateNonceAsync(),
                    RequestedAttributes = new Dictionary<string, ProofAttributeInfo>
                    {
                        { "id-verification", new ProofAttributeInfo { Names = new [] { "name", "age" } } }
                    }
                });
            
            var proofRecordHolder = await _fixture.HolderProofService.ProcessRequestAsync(_fixture.HolderAgentContext, requestPresentationMessage, _fixture.PairedAgents.Connection2);
            var availableCredentials = await _fixture.HolderProofService.ListCredentialsForProofRequestAsync(_fixture.HolderAgentContext, proofRecordHolder.RequestJson.ToObject<ProofRequest>(), "id-verification");

            var (presentationMessage, _) = await _fixture.HolderProofService.CreatePresentationAsync(
                _fixture.HolderAgentContext, proofRecordHolder.Id, new RequestedCredentials
                {
                    RequestedAttributes = new Dictionary<string, RequestedAttribute>
                    {
                        { "id-verification", new RequestedAttribute 
                            {
                                CredentialId = availableCredentials.First().CredentialInfo.Referent,
                                Revealed = true
                            }
                        }
                    }
                });
            
            proofRecordIssuer = await _fixture.IssuerProofService.ProcessPresentationAsync(_fixture.IssuerAgentContext, presentationMessage);
            var valid = await _fixture.IssuerProofService.VerifyProofAsync(_fixture.IssuerAgentContext, proofRecordIssuer.Id);
        
            Assert.True(valid);
            Assert.False(await _fixture.HolderProofService.IsRevokedAsync(_fixture.HolderAgentContext, availableCredentials.First().CredentialInfo.Referent));
        }

        [Fact(DisplayName = "Test verification with NonRevoked set on proof request level")]
        public async Task CanVerifyWithNonRevokedSetOnProofRequestLevel()
        {
            var (offer, record) = await _fixture.IssuerCredentialService
                .CreateOfferAsync(_fixture.IssuerAgentContext, new OfferConfiguration
                {
                    CredentialDefinitionId = _fixture.RevocableCredentialDefinitionId,
                    IssuerDid = _fixture.IssuerConfiguration.IssuerDid,
                    CredentialAttributeValues = new[]
                    {
                        new CredentialPreviewAttribute("name", "random"),
                        new CredentialPreviewAttribute("age", "22")
                    }
                });
            await _fixture.IssuerMessageService.SendAsync(_fixture.IssuerAgentContext, offer, _fixture.PairedAgents.Connection1);
            
            var credentialRecordOnHolderSide = (await _fixture.HolderCredentialService.ListAsync(_fixture.HolderAgentContext))
                .First(credentialRecord => credentialRecord.State == CredentialState.Offered);
            var (request, _) = await _fixture.HolderCredentialService.CreateRequestAsync(_fixture.HolderAgentContext, credentialRecordOnHolderSide.Id);
            await _fixture.HolderMessageService.SendAsync(_fixture.HolderAgentContext, request, _fixture.PairedAgents.Connection2);

            var credentialRecordOnIssuerSide = (await _fixture.IssuerCredentialService.ListRequestsAsync(
                _fixture.IssuerAgentContext)).First();
            var (issuance, _) = await _fixture.IssuerCredentialService.CreateCredentialAsync(_fixture.IssuerAgentContext, credentialRecordOnIssuerSide.Id);
            await _fixture.IssuerMessageService.SendAsync(_fixture.IssuerAgentContext, issuance, _fixture.PairedAgents.Connection1);
            
            credentialRecordOnHolderSide =
                await _fixture.HolderCredentialService.GetAsync(_fixture.HolderAgentContext,
                    credentialRecordOnHolderSide.Id);
            credentialRecordOnIssuerSide =
                await _fixture.IssuerCredentialService.GetAsync(_fixture.IssuerAgentContext,
                    credentialRecordOnIssuerSide.Id);
            
            Assert.Equal(CredentialState.Issued, credentialRecordOnHolderSide.State);
            Assert.Equal(CredentialState.Issued, credentialRecordOnIssuerSide.State);
            
            var (requestPresentationMessage, proofRecordIssuer) = await _fixture.IssuerProofService
                .CreateRequestAsync(_fixture.IssuerAgentContext, new ProofRequest
                {
                    Name = "Test Verification",
                    Version = "1.0",
                    Nonce = await AnonCreds.GenerateNonceAsync(),
                    RequestedAttributes = new Dictionary<string, ProofAttributeInfo>
                    {
                        { "id-verification", new ProofAttributeInfo { Names = new [] { "name", "age" } } }
                    },
                    NonRevoked = new RevocationInterval
                    {
                        From = 0,
                        To = _now
                    }
                });
        
            var proofRecordHolder = await _fixture.HolderProofService.ProcessRequestAsync(_fixture.HolderAgentContext, requestPresentationMessage, _fixture.PairedAgents.Connection2);
            var availableCredentials = await _fixture.HolderProofService
                .ListCredentialsForProofRequestAsync(_fixture.HolderAgentContext, proofRecordHolder.RequestJson.ToObject<ProofRequest>(), "id-verification");
            
            var (presentationMessage, _) = await _fixture.HolderProofService.CreatePresentationAsync(
                _fixture.HolderAgentContext, proofRecordHolder.Id, new RequestedCredentials
                {
                    RequestedAttributes = new Dictionary<string, RequestedAttribute>
                    {
                        { "id-verification", new RequestedAttribute
                            {
                                CredentialId = availableCredentials.First().CredentialInfo.Referent,
                                Revealed = true
                            }
                        }
                    }
                });
            proofRecordIssuer = await _fixture.IssuerProofService.ProcessPresentationAsync(_fixture.IssuerAgentContext, presentationMessage);
        
            var valid = await _fixture.IssuerProofService.VerifyProofAsync(_fixture.IssuerAgentContext, proofRecordIssuer.Id);
            Assert.True(valid);
        }
        
        [Fact(DisplayName = "Test verification with NonRevoked set on attribute level")]
        public async Task CanVerifyWithNonRevokedSetOnAttributeLevel()
        {
            var (offer, record) = await _fixture.IssuerCredentialService
                .CreateOfferAsync(_fixture.IssuerAgentContext, new OfferConfiguration
                {
                    CredentialDefinitionId = _fixture.RevocableCredentialDefinitionId,
                    IssuerDid = _fixture.IssuerConfiguration.IssuerDid,
                    CredentialAttributeValues = new[]
                    {
                        new CredentialPreviewAttribute("name", "random"),
                        new CredentialPreviewAttribute("age", "22")
                    }
                });
            await _fixture.IssuerMessageService.SendAsync(_fixture.IssuerAgentContext, offer, _fixture.PairedAgents.Connection1);
            
            var credentialRecordOnHolderSide = (await _fixture.HolderCredentialService.ListAsync(_fixture.HolderAgentContext))
                .First(credentialRecord => credentialRecord.State == CredentialState.Offered);
            var (request, _) = await _fixture.HolderCredentialService.CreateRequestAsync(_fixture.HolderAgentContext, credentialRecordOnHolderSide.Id);
            await _fixture.HolderMessageService.SendAsync(_fixture.HolderAgentContext, request, _fixture.PairedAgents.Connection2);

            var credentialRecordOnIssuerSide = (await _fixture.IssuerCredentialService.ListRequestsAsync(
                _fixture.IssuerAgentContext)).First();
            var (issuance, _) = await _fixture.IssuerCredentialService.CreateCredentialAsync(_fixture.IssuerAgentContext, credentialRecordOnIssuerSide.Id);
            await _fixture.IssuerMessageService.SendAsync(_fixture.IssuerAgentContext, issuance, _fixture.PairedAgents.Connection1);
            
            credentialRecordOnHolderSide =
                await _fixture.HolderCredentialService.GetAsync(_fixture.HolderAgentContext,
                    credentialRecordOnHolderSide.Id);
            credentialRecordOnIssuerSide =
                await _fixture.IssuerCredentialService.GetAsync(_fixture.IssuerAgentContext,
                    credentialRecordOnIssuerSide.Id);
            
            Assert.Equal(CredentialState.Issued, credentialRecordOnHolderSide.State);
            Assert.Equal(CredentialState.Issued, credentialRecordOnIssuerSide.State);
            
            var (requestPresentationMessage, proofRecordIssuer) = await _fixture.IssuerProofService
                .CreateRequestAsync(_fixture.IssuerAgentContext, new ProofRequest
                {
                    Name = "Test Verification",
                    Version = "1.0",
                    Nonce = await AnonCreds.GenerateNonceAsync(),
                    RequestedAttributes = new Dictionary<string, ProofAttributeInfo>
                    {
                        { "id-verification", new ProofAttributeInfo
                            {
                                Names = new [] { "name", "age" },
                                NonRevoked = new RevocationInterval
                                {
                                    From = 0,
                                    To = _now
                                }            
                            } 
                        }
                    }
                });

            var proofRecordHolder = await _fixture.HolderProofService.ProcessRequestAsync(_fixture.HolderAgentContext, requestPresentationMessage, _fixture.PairedAgents.Connection2);
            var availableCredentials = await _fixture.HolderProofService
                .ListCredentialsForProofRequestAsync(_fixture.HolderAgentContext, proofRecordHolder.RequestJson.ToObject<ProofRequest>(), "id-verification");  
    
            var(presentationMessage, _) = await _fixture.HolderProofService.CreatePresentationAsync(
                _fixture.HolderAgentContext, proofRecordHolder.Id, new RequestedCredentials
                {
                    RequestedAttributes = new Dictionary<string, RequestedAttribute>
                    {
                        { "id-verification", new RequestedAttribute
                            {
                                CredentialId = availableCredentials.First().CredentialInfo.Referent,
                                Revealed = true
                            }
                        }
                    }
                });

            proofRecordIssuer = await _fixture.IssuerProofService.ProcessPresentationAsync(_fixture.IssuerAgentContext, presentationMessage);

            var valid = await _fixture.IssuerProofService.VerifyProofAsync(_fixture.IssuerAgentContext, proofRecordIssuer.Id);
            Assert.True(valid);
        }
    }
}
