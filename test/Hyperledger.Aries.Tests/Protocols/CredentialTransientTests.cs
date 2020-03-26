using System.Threading.Tasks;
using Hyperledger.TestHarness;
using Hyperledger.TestHarness.Mock;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System;
using Hyperledger.Aries.Decorators.Service;
using Hyperledger.Aries.Decorators;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Configuration;

namespace Hyperledger.Aries.Tests.Protocols
{
    public class CredentialTransientTests : TestSingleWallet
    {
        static CredentialTransientTests()
        {
            global::Hyperledger.Aries.Utils.Runtime.SetFlags(Hyperledger.Aries.Utils.Runtime.LedgerLookupRetryFlag);
        }

        [Fact(DisplayName = "Issue credential over connectionless transport using protocol v1")]
        public async Task IssueCredentialOverConnectionlessTransport()
        {
            var agents = await InProcAgent.CreatePairedAsync(false);
            var issuerProvisioningService = agents.Agent1.Host.Services.GetService<IProvisioningService>();
            var issuerSchemaService = agents.Agent1.Host.Services.GetService<ISchemaService>();
            var issuerCredentialService = agents.Agent1.Host.Services.GetService<ICredentialService>();

            var issuerProvisioning = await issuerProvisioningService.GetProvisioningAsync(agents.Agent1.Context.Wallet);
            await PromoteTrustAnchor(issuerProvisioning.IssuerDid, issuerProvisioning.IssuerVerkey);
            
            var schemaId = await issuerSchemaService.CreateSchemaAsync(
                context: agents.Agent1.Context,
                issuerDid: issuerProvisioning.IssuerDid,
                name: $"test-schema-{Guid.NewGuid()}",
                version: "1.0",
                attributeNames: new[] { "test-attr" });

            var credentialDefinitionId = await issuerSchemaService.CreateCredentialDefinitionAsync(
                context: agents.Agent1.Context,
                schemaId: schemaId,
                issuerDid: issuerProvisioning.IssuerDid,
                tag: "default",
                supportsRevocation: false,
                maxCredentialCount: 0,
                tailsBaseUri: new Uri("https://test"));

            var (offerMessage, issuerRecord) = await issuerCredentialService.CreateOfferAsync(agents.Agent1.Context, new OfferConfiguration
            {
                CredentialDefinitionId = credentialDefinitionId,
                CredentialAttributeValues = new [] { new CredentialPreviewAttribute("test-attr", "test-value") }
            });

            Assert.NotNull(offerMessage.FindDecorator<ServiceDecorator>(DecoratorNames.ServiceDecorator));
            Assert.Equal(CredentialState.Offered, issuerRecord.State);
            Assert.Null(issuerRecord.ConnectionId);

            var holderCredentialService = agents.Agent2.Host.Services.GetService<ICredentialService>();

            var holderRecord = await holderCredentialService.CreateCredentialAsync(agents.Agent2.Context, offerMessage);
            issuerRecord = await issuerCredentialService.GetAsync(agents.Agent1.Context, issuerRecord.Id);

            Assert.NotNull(holderRecord);
            Assert.Equal(expected: CredentialState.Issued, actual: holderRecord.State);
            Assert.Equal(expected: CredentialState.Issued, actual: issuerRecord.State);
            Assert.NotNull(holderRecord.CredentialAttributesValues);
            Assert.Null(holderRecord.ConnectionId);
        }

        [Fact(DisplayName = "Create a credential and automatically scale revocation registry")]
        public async Task CreateCredentialAndAutoScaleRevocationRegistry()
        {
            var agents = await InProcAgent.CreatePairedAsync(false);
            var issuerProvisioningService = agents.Agent1.Host.Services.GetService<IProvisioningService>();
            var issuerSchemaService = agents.Agent1.Host.Services.GetService<ISchemaService>();
            var issuerCredentialService = agents.Agent1.Host.Services.GetService<ICredentialService>();

            var issuerProvisioning = await issuerProvisioningService.GetProvisioningAsync(agents.Agent1.Context.Wallet);
            await PromoteTrustAnchor(issuerProvisioning.IssuerDid, issuerProvisioning.IssuerVerkey);

            var schemaId = await issuerSchemaService.CreateSchemaAsync(
                context: agents.Agent1.Context,
                issuerDid: issuerProvisioning.IssuerDid,
                name: $"test-schema-{Guid.NewGuid()}",
                version: "1.0",
                attributeNames: new[] { "test-attr" });

            string revocationRegistryId1 = null;
            string revocationRegistryId2 = null;
            string revocationRegistryId3 = null;

            var credentialDefinitionId = await issuerSchemaService.CreateCredentialDefinitionAsync(
                context: agents.Agent1.Context,
                new CredentialDefinitionConfiguration
                {
                    SchemaId = schemaId,
                    EnableRevocation = true,
                    RevocationRegistrySize = 1,
                    RevocationRegistryBaseUri = "https://test"
                });

            // First credential - will max out the registry
            {
                var (offerMessage, issuerRecord) = await issuerCredentialService.CreateOfferAsync(agents.Agent1.Context, new OfferConfiguration
                {
                    CredentialDefinitionId = credentialDefinitionId,
                    CredentialAttributeValues = new[] { new CredentialPreviewAttribute("test-attr", "test-value") }
                });

                Assert.NotNull(offerMessage.FindDecorator<ServiceDecorator>(DecoratorNames.ServiceDecorator));
                Assert.Equal(CredentialState.Offered, issuerRecord.State);
                Assert.Null(issuerRecord.ConnectionId);

                var holderCredentialService = agents.Agent2.Host.Services.GetService<ICredentialService>();

                var holderRecord = await holderCredentialService.CreateCredentialAsync(agents.Agent2.Context, offerMessage);
                issuerRecord = await issuerCredentialService.GetAsync(agents.Agent1.Context, issuerRecord.Id);
                var definitionRecord = await agents.Agent1.Host.Services.GetService<ISchemaService>().GetCredentialDefinitionAsync(agents.Agent1.Context.Wallet, credentialDefinitionId);

                Assert.NotNull(holderRecord);
                Assert.Equal(expected: CredentialState.Issued, actual: holderRecord.State);
                Assert.Equal(expected: CredentialState.Issued, actual: issuerRecord.State);
                Assert.NotNull(holderRecord.CredentialAttributesValues);
                Assert.Null(holderRecord.ConnectionId);
                Assert.Equal(definitionRecord.CurrentRevocationRegistryId, issuerRecord.RevocationRegistryId);
                Assert.Equal(definitionRecord.CurrentRevocationRegistryId, holderRecord.RevocationRegistryId);

                revocationRegistryId1 = definitionRecord.CurrentRevocationRegistryId;
            }

            // Second credential, will auto scale registry
            {
                var (offerMessage, issuerRecord) = await issuerCredentialService.CreateOfferAsync(agents.Agent1.Context, new OfferConfiguration
                {
                    CredentialDefinitionId = credentialDefinitionId,
                    CredentialAttributeValues = new[] { new CredentialPreviewAttribute("test-attr", "test-value") }
                });

                Assert.NotNull(offerMessage.FindDecorator<ServiceDecorator>(DecoratorNames.ServiceDecorator));
                Assert.Equal(CredentialState.Offered, issuerRecord.State);
                Assert.Null(issuerRecord.ConnectionId);

                var holderCredentialService = agents.Agent2.Host.Services.GetService<ICredentialService>();

                var holderRecord = await holderCredentialService.CreateCredentialAsync(agents.Agent2.Context, offerMessage);
                issuerRecord = await issuerCredentialService.GetAsync(agents.Agent1.Context, issuerRecord.Id);
                var definitionRecord = await agents.Agent1.Host.Services.GetService<ISchemaService>().GetCredentialDefinitionAsync(agents.Agent1.Context.Wallet, credentialDefinitionId);

                Assert.NotNull(holderRecord);
                Assert.Equal(expected: CredentialState.Issued, actual: holderRecord.State);
                Assert.Equal(expected: CredentialState.Issued, actual: issuerRecord.State);
                Assert.NotNull(holderRecord.CredentialAttributesValues);
                Assert.Null(holderRecord.ConnectionId);
                Assert.Equal(definitionRecord.CurrentRevocationRegistryId, issuerRecord.RevocationRegistryId);
                Assert.Equal(definitionRecord.CurrentRevocationRegistryId, holderRecord.RevocationRegistryId);

                revocationRegistryId2 = definitionRecord.CurrentRevocationRegistryId;
            }

            // Third credential, will auto scale registry
            {
                var (offerMessage, issuerRecord) = await issuerCredentialService.CreateOfferAsync(agents.Agent1.Context, new OfferConfiguration
                {
                    CredentialDefinitionId = credentialDefinitionId,
                    CredentialAttributeValues = new[] { new CredentialPreviewAttribute("test-attr", "test-value") }
                });

                Assert.NotNull(offerMessage.FindDecorator<ServiceDecorator>(DecoratorNames.ServiceDecorator));
                Assert.Equal(CredentialState.Offered, issuerRecord.State);
                Assert.Null(issuerRecord.ConnectionId);

                var holderCredentialService = agents.Agent2.Host.Services.GetService<ICredentialService>();

                var holderRecord = await holderCredentialService.CreateCredentialAsync(agents.Agent2.Context, offerMessage);
                issuerRecord = await issuerCredentialService.GetAsync(agents.Agent1.Context, issuerRecord.Id);
                var definitionRecord = await agents.Agent1.Host.Services.GetService<ISchemaService>().GetCredentialDefinitionAsync(agents.Agent1.Context.Wallet, credentialDefinitionId);

                Assert.NotNull(holderRecord);
                Assert.Equal(expected: CredentialState.Issued, actual: holderRecord.State);
                Assert.Equal(expected: CredentialState.Issued, actual: issuerRecord.State);
                Assert.NotNull(holderRecord.CredentialAttributesValues);
                Assert.Null(holderRecord.ConnectionId);
                Assert.Equal(definitionRecord.CurrentRevocationRegistryId, issuerRecord.RevocationRegistryId);
                Assert.Equal(definitionRecord.CurrentRevocationRegistryId, holderRecord.RevocationRegistryId);

                revocationRegistryId3 = definitionRecord.CurrentRevocationRegistryId;
            }

            Assert.NotEqual(revocationRegistryId1, revocationRegistryId2);
            Assert.NotEqual(revocationRegistryId2, revocationRegistryId3);
            Assert.NotEqual(revocationRegistryId1, revocationRegistryId3);
        }
    }
}