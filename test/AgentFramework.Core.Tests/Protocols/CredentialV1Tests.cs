using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.TestHarness;
using AgentFramework.TestHarness.Mock;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System;
using AgentFramework.Core.Models.Credentials;
using System.Linq;
using AgentFramework.Core.Models.Records;

namespace AgentFramework.Core.Tests.Protocols
{
    public class CredentialV1Tests : TestSingleWallet
    {
        [Fact(DisplayName = "Test Credential Issuance Protocol v1.0")]
        public async Task TestCredentialIssuanceV1()
        {
            var pair = await InProcAgent.CreatePairedAsync(true);

            // Configure agent1 as issuer
            var issuerConfiguration = await pair.Agent1.Provider.GetRequiredService<IProvisioningService>()
                .GetProvisioningAsync(pair.Agent1.Context.Wallet);
            await PromoteTrustAnchor(issuerConfiguration.IssuerDid, issuerConfiguration.IssuerVerkey);

            var schemaId = await pair.Agent1.Provider.GetRequiredService<ISchemaService>()
                .CreateSchemaAsync(
                    context: pair.Agent1.Context,
                    issuerDid: issuerConfiguration.IssuerDid,
                    name: $"test-schema-{Guid.NewGuid().ToString()}",
                    version: "1.0",
                    attributeNames: new[] { "name" });

            var definitionId = await pair.Agent1.Provider.GetRequiredService<ISchemaService>()
                .CreateCredentialDefinitionAsync(
                    context: pair.Agent1.Context,
                    schemaId: schemaId,
                    issuerDid: issuerConfiguration.IssuerDid,
                    tag: "tag",
                    supportsRevocation: false,
                    maxCredentialCount: 0,
                    tailsBaseUri: new Uri("http://localhost"));

            // Send offer
            var (offer, record) = await pair.Agent1.Provider.GetRequiredService<ICredentialService>()
                .CreateOfferV1Async(pair.Agent1.Context, new OfferConfiguration
                {
                    CredentialDefinitionId = definitionId,
                    IssuerDid = issuerConfiguration.IssuerDid,
                    CredentialAttributeValues = new []
                    {
                        new CredentialPreviewAttribute
                        {
                            Name = "name",
                            Value = "random",
                            MimeType = CredentialMimeTypes.TextMimeType
                        }
                    }
                });
            await pair.Agent1.Provider.GetRequiredService<IMessageService>()
                .SendAsync(pair.Agent1.Context.Wallet, offer, pair.Connection1);

            // Find credential for Agent 2
            var credentials = await pair.Agent2.Provider.GetService<ICredentialService>()
                .ListAsync(pair.Agent2.Context);
            var credential = credentials.First();

            // Accept the offer and send request
            var (request, _) = await pair.Agent2.Provider.GetService<ICredentialService>()
                .CreateRequestAsync(pair.Agent2.Context, credential.Id);
            await pair.Agent2.Provider.GetService<IMessageService>()
                .SendAsync(pair.Agent2.Context.Wallet, request, pair.Connection2);

            // Issue credential
            var (issue, _) = await pair.Agent1.Provider.GetRequiredService<ICredentialService>()
                .CreateCredentialAsync(pair.Agent1.Context, record.Id);
            await pair.Agent1.Provider.GetService<IMessageService>()
                .SendAsync(pair.Agent1.Context.Wallet, issue, pair.Connection1);

            // Assert
            var credentialHolder = await pair.Agent2.Provider.GetService<ICredentialService>()
                .GetAsync(pair.Agent2.Context, credential.Id);
            var credentialIssuer = await pair.Agent1.Provider.GetService<ICredentialService>()
                .GetAsync(pair.Agent1.Context, record.Id);

            Assert.Equal(CredentialState.Issued, credentialHolder.State);
            Assert.Equal(CredentialState.Issued, credentialIssuer.State);
        }
    }
}