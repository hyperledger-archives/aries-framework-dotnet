using System;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.TestHarness;
using Newtonsoft.Json.Linq;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace AgentFramework.Core.Tests
{
    public class SchemaServiceTests : TestSingleWallet
    {
        [Fact]
        public async Task CanCreateAndResolveSchema()
        {
            var schemaService = Host.Services.GetService<ISchemaService>();
            var record = await provisioningService.GetProvisioningAsync(Context.Wallet);

            await PromoteTrustAnchor(record.IssuerDid, record.IssuerVerkey);

            var schemaName = $"Test-Schema-{Guid.NewGuid().ToString("N")}";
            var schemaVersion = "1.0";
            var schemaAttrNames = new[] {"test_attr_1", "test_attr_2"};

            //Create a dummy schema
            var schemaId = await schemaService.CreateSchemaAsync(Context, record.IssuerDid,
                schemaName, schemaVersion, schemaAttrNames);

            //Resolve it from the ledger with its identifier
            var resultSchema = await schemaService.LookupSchemaAsync(await Context.Pool, schemaId);

            var resultSchemaName = JObject.Parse(resultSchema)["name"].ToString();
            var resultSchemaVersion = JObject.Parse(resultSchema)["version"].ToString();
            var sequenceId = Convert.ToInt32(JObject.Parse(resultSchema)["seqNo"].ToString());

            Assert.Equal(schemaName, resultSchemaName);
            Assert.Equal(schemaVersion, resultSchemaVersion);

            //Resolve it from the ledger with its sequence Id
            var secondResultSchema = await schemaService.LookupSchemaAsync(await Context.Pool, sequenceId);

            var secondResultSchemaName = JObject.Parse(secondResultSchema)["name"].ToString();
            var secondResultSchemaVersion = JObject.Parse(secondResultSchema)["version"].ToString();

            Assert.Equal(schemaName, secondResultSchemaName);
            Assert.Equal(schemaVersion, secondResultSchemaVersion);
        }

        [Fact]
        public async Task CanCreateAndResolveCredentialDefinitionAndSchema()
        {
            var schemaService = Host.Services.GetService<ISchemaService>();
            var provisioningService = Host.Services.GetService<IProvisioningService>();

            var record = await provisioningService.GetProvisioningAsync(Context.Wallet);

            await PromoteTrustAnchor(record.IssuerDid, record.IssuerVerkey);

            var schemaName = $"Test-Schema-{Guid.NewGuid().ToString()}";
            var schemaVersion = "1.0";
            var schemaAttrNames = new[] { "test_attr_1", "test_attr_2" };

            //Create a dummy schema
            var schemaId = await schemaService.CreateSchemaAsync(Context, record.IssuerDid,
                schemaName, schemaVersion, schemaAttrNames);

            var credId = await schemaService.CreateCredentialDefinitionAsync(Context, schemaId,
                record.IssuerDid, "Tag", false, 100, new Uri("http://mock/tails"));

            var credDef =
                await schemaService.LookupCredentialDefinitionAsync(await Context.Pool, credId);

            var resultCredId = JObject.Parse(credDef)["id"].ToString();

            Assert.Equal(credId, resultCredId);

            var result = await schemaService.LookupSchemaFromCredentialDefinitionAsync(await Context.Pool, credId);

            var resultSchemaName = JObject.Parse(result)["name"].ToString();
            var resultSchemaVersion = JObject.Parse(result)["version"].ToString();

            Assert.Equal(schemaName, resultSchemaName);
            Assert.Equal(schemaVersion, resultSchemaVersion);

            var recordResult = await schemaService.GetCredentialDefinitionAsync(Context.Wallet, credId);

            Assert.Equal(schemaId, recordResult.SchemaId);
        }
    }
}
