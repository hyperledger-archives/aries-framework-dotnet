using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Features.PresentProof;
using Hyperledger.Indy.AnonCredsApi;
using Hyperledger.TestHarness;
using Hyperledger.TestHarness.Mock;
using Hyperledger.Aries.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Hyperledger.Aries.Tests.Protocols
{
    public class RevocationTests : TestSingleWallet
    {
        [Fact(DisplayName = "Test Proof Presentation Protocol with revocation")]
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
                    name: $"test-schema-{Guid.NewGuid()}",
                    version: "1.0",
                    attributeNames: new[] { "name", "age" });

            var definitionWithRevocationId = await pair.Agent1.Provider.GetRequiredService<ISchemaService>()
                .CreateCredentialDefinitionAsync(
                    context: pair.Agent1.Context,
                    new CredentialDefinitionConfiguration
                    {
                        SchemaId = schemaId,
                        EnableRevocation = true,
                        RevocationRegistryBaseUri = "http://localhost",
                        Tag = "revoc"
                    });

            var definitionId = await pair.Agent1.Provider.GetRequiredService<ISchemaService>()
                .CreateCredentialDefinitionAsync(
                    context: pair.Agent1.Context,
                    new CredentialDefinitionConfiguration
                    {
                        SchemaId = schemaId,
                        EnableRevocation = false,
                        RevocationRegistryBaseUri = "http://localhost",
                        Tag = "norevoc"
                    });

            // Send offer for two credentials
            var (offer, record) = await pair.Agent1.Provider.GetRequiredService<ICredentialService>()
                .CreateOfferAsync(pair.Agent1.Context, new OfferConfiguration
                {
                    CredentialDefinitionId = definitionWithRevocationId,
                    IssuerDid = issuerConfiguration.IssuerDid,
                    CredentialAttributeValues = new[]
                    {
                        new CredentialPreviewAttribute("name", "random"),
                        new CredentialPreviewAttribute("age", "22")
                    }
                });
            await pair.Agent1.Provider.GetRequiredService<IMessageService>()
                .SendAsync(pair.Agent1.Context.Wallet, offer, pair.Connection1);

            var issuerCredentialWithRevocation = record;

            (offer, record) = await pair.Agent1.Provider.GetRequiredService<ICredentialService>()
               .CreateOfferAsync(pair.Agent1.Context, new OfferConfiguration
               {
                   CredentialDefinitionId = definitionId,
                   IssuerDid = issuerConfiguration.IssuerDid,
                   CredentialAttributeValues = new[]
                   {
                        new CredentialPreviewAttribute("name", "random"),
                        new CredentialPreviewAttribute("age", "22")
                   }
               });
            await pair.Agent1.Provider.GetRequiredService<IMessageService>()
                .SendAsync(pair.Agent1.Context.Wallet, offer, pair.Connection1);

            // Find credential for Agent 2 and accept all offers
            var credentials = await pair.Agent2.Provider.GetService<ICredentialService>()
                .ListAsync(pair.Agent2.Context);
            foreach (var credential in credentials.Where(x => x.State == CredentialState.Offered))
            {
                var (request, _) = await pair.Agent2.Provider.GetService<ICredentialService>()
                .CreateRequestAsync(pair.Agent2.Context, credential.Id);
                await pair.Agent2.Provider.GetService<IMessageService>()
                    .SendAsync(pair.Agent2.Context.Wallet, request, pair.Connection2);
            }

            // Issue credential
            credentials = await pair.Agent1.Provider.GetService<ICredentialService>()
                .ListRequestsAsync(pair.Agent1.Context);
            foreach (var credential in credentials)
            {
                var (issue, _) = await pair.Agent1.Provider.GetRequiredService<ICredentialService>()
                .CreateCredentialAsync(pair.Agent1.Context, credential.Id);
                await pair.Agent1.Provider.GetService<IMessageService>()
                    .SendAsync(pair.Agent1.Context.Wallet, issue, pair.Connection1);
            }

            // Assert
            foreach (var credential in await pair.Agent1.Provider.GetService<ICredentialService>()
                .ListAsync(pair.Agent1.Context))
            {
                Assert.Equal(CredentialState.Issued, credential.State);
            }
            foreach (var credential in await pair.Agent2.Provider.GetService<ICredentialService>()
                 .ListAsync(pair.Agent2.Context))
            {
                Assert.Equal(CredentialState.Issued, credential.State);
            }


            // Verification - without revocation
            var (requestPresentationMessage, proofRecordIssuer) = await pair.Agent1.Provider.GetService<IProofService>()
                .CreateRequestAsync(pair.Agent1.Context, new ProofRequest
                {
                    Name = "Test Verification",
                    Version = "1.0",
                    Nonce = await AnonCreds.GenerateNonceAsync(),
                    RequestedAttributes = new Dictionary<string, ProofAttributeInfo>
                    {
                        { "id-verification", new ProofAttributeInfo { Names = new [] { "name", "age" } } }
                    }
                });

            var proofRecordHolder = await pair.Agent2.Provider.GetService<IProofService>()
                .ProcessRequestAsync(pair.Agent2.Context, requestPresentationMessage, pair.Connection2);

            var availableCredentials = await pair.Agent2.Provider.GetService<IProofService>()
                .ListCredentialsForProofRequestAsync(pair.Agent2.Context, proofRecordHolder.RequestJson.ToObject<ProofRequest>(), "id-verification");
            
            var (presentationMessage, _) = await pair.Agent2.Provider.GetService<IProofService>()
                .CreatePresentationAsync(pair.Agent2.Context, proofRecordHolder.Id, new RequestedCredentials
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

            proofRecordIssuer = await pair.Agent1.Provider.GetService<IProofService>()
                .ProcessPresentationAsync(pair.Agent1.Context, presentationMessage);

            var valid = await pair.Agent1.Provider.GetService<IProofService>()
                .VerifyProofAsync(pair.Agent1.Context, proofRecordIssuer.Id);

            Assert.True(valid);

            // Verification - with revocation
            var now = (uint)DateTimeOffset.Now.ToUnixTimeSeconds();

            (requestPresentationMessage, proofRecordIssuer) = await pair.Agent1.Provider.GetService<IProofService>()
                .CreateRequestAsync(pair.Agent1.Context, new ProofRequest
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
                        From = (uint)DateTimeOffset.Now.AddSeconds(-2).ToUnixTimeSeconds(),
                        To = now
                    }
                });

            proofRecordHolder = await pair.Agent2.Provider.GetService<IProofService>()
                .ProcessRequestAsync(pair.Agent2.Context, requestPresentationMessage, pair.Connection2);
            availableCredentials = await pair.Agent2.Provider.GetService<IProofService>()
                .ListCredentialsForProofRequestAsync(pair.Agent2.Context, proofRecordHolder.RequestJson.ToObject<ProofRequest>(), "id-verification");

            (presentationMessage, _) = await pair.Agent2.Provider.GetService<IProofService>()
                .CreatePresentationAsync(pair.Agent2.Context, proofRecordHolder.Id, new RequestedCredentials
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

            proofRecordIssuer = await pair.Agent1.Provider.GetService<IProofService>()
                .ProcessPresentationAsync(pair.Agent1.Context, presentationMessage);

            valid = await pair.Agent1.Provider.GetService<IProofService>()
                .VerifyProofAsync(pair.Agent1.Context, proofRecordIssuer.Id);

            Assert.True(valid);

            // Revoke the credential

            await pair.Agent1.Provider.GetService<ICredentialService>()
               .RevokeCredentialAsync(pair.Agent1.Context, issuerCredentialWithRevocation.Id);

            now = (uint)DateTimeOffset.Now.AddYears(1).AddDays(1).ToUnixTimeSeconds();

            (requestPresentationMessage, proofRecordIssuer) = await pair.Agent1.Provider.GetService<IProofService>()
                .CreateRequestAsync(pair.Agent1.Context, new ProofRequest
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
                        From = (uint)DateTimeOffset.Now.AddYears(1).ToUnixTimeSeconds(),
                        To = now
                    }
                });

            proofRecordHolder = await pair.Agent2.Provider.GetService<IProofService>()
                .ProcessRequestAsync(pair.Agent2.Context, requestPresentationMessage, pair.Connection2);
            availableCredentials = await pair.Agent2.Provider.GetService<IProofService>()
                .ListCredentialsForProofRequestAsync(pair.Agent2.Context, proofRecordHolder.RequestJson.ToObject<ProofRequest>(), "id-verification");

            (presentationMessage, _) = await pair.Agent2.Provider.GetService<IProofService>()
                .CreatePresentationAsync(pair.Agent2.Context, proofRecordHolder.Id, new RequestedCredentials
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

            proofRecordIssuer = await pair.Agent1.Provider.GetService<IProofService>()
                .ProcessPresentationAsync(pair.Agent1.Context, presentationMessage);

            valid = await pair.Agent1.Provider.GetService<IProofService>()
                .VerifyProofAsync(pair.Agent1.Context, proofRecordIssuer.Id);

            //Assert.False(valid);
        }
    }
}
