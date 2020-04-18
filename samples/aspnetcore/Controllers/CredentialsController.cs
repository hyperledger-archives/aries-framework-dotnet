using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.DidExchange;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Models.Records;
using Hyperledger.Aries.Storage;
using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.LedgerApi;
using Newtonsoft.Json;
using WebAgent.Models;

namespace WebAgent.Controllers
{
    public class CredentialsController : Controller
    {
        private readonly IAgentProvider _agentContextProvider;
        private readonly IProvisioningService _provisionService;
        private readonly IWalletService _walletService;
        private readonly IConnectionService _connectionService;
        private readonly ICredentialService _credentialService;
        private readonly ISchemaService _schemaService;
        private readonly IMessageService _messageService;

        public CredentialsController(
            IAgentProvider agentContextProvider,
            IProvisioningService provisionService,
            IWalletService walletService,
            IConnectionService connectionService,
            ICredentialService credentialService,
            ISchemaService schemaService,
            IMessageService messageService)
        {
            _agentContextProvider = agentContextProvider;
            _provisionService = provisionService;
            _walletService = walletService;
            _connectionService = connectionService;
            _credentialService = credentialService;
            _schemaService = schemaService;
            _messageService = messageService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var context = await _agentContextProvider.GetContextAsync();
            var credentials = await _credentialService.ListAsync(context);
            var models = new List<CredentialViewModel>();
            foreach ( var c in credentials) {
                models.Add(new CredentialViewModel{
                    SchemaId = c.SchemaId,
                    CreatedAt = c.CreatedAtUtc ?? DateTime.MinValue,
                    State = c.State,
                    CredentialAttributesValues = c.CredentialAttributesValues
                });
            }
            return View(new CredentialsViewModel{ Credentials = models });
        }

        [HttpGet]
        public async Task<IActionResult> RegisterSchema()
        {
            var context = await _agentContextProvider.GetContextAsync();
            var issuer = await _provisionService.GetProvisioningAsync(context.Wallet);
            var Trustee = await Did.CreateAndStoreMyDidAsync(context.Wallet,
                new { seed = "000000000000000000000000Trustee1" }.ToJson());
            await Ledger.SignAndSubmitRequestAsync(await context.Pool, context.Wallet, Trustee.Did,
                await Ledger.BuildNymRequestAsync(Trustee.Did, issuer.IssuerDid, issuer.IssuerVerkey, null, "ENDORSER"));

            var schemaId = await _schemaService.CreateSchemaAsync(
                context: context,
                issuerDid: issuer.IssuerDid,
                name: "degree-schema",
                version: "1.0",
                attributeNames: new[] { "name", "date", "degree", "age", "timestamp" });
            await _schemaService.CreateCredentialDefinitionAsync(context, new CredentialDefinitionConfiguration{
                SchemaId = schemaId,
                Tag = "default",
                EnableRevocation = false,
                RevocationRegistrySize = 0,
                RevocationRegistryBaseUri = "",
                RevocationRegistryAutoScale = false,
                IssuerDid = issuer.IssuerDid});

            return RedirectToAction("CredentialsForm");
        }

        [HttpGet]
        public async Task<IActionResult> CredentialsForm()
        {
            var context = await _agentContextProvider.GetContextAsync();
            var model = new CredentialFormModel
            {
                Connections = await _connectionService.ListAsync(context),
                CredentialDefinitions = await _schemaService.ListCredentialDefinitionsAsync(context.Wallet),
                Schemas = await _schemaService.ListSchemasAsync(context.Wallet)
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> IssueCredentials(CredentialOfferModel model)
        {
            var context = await _agentContextProvider.GetContextAsync();
            var issuer = await _provisionService.GetProvisioningAsync(context.Wallet);
            var connection = await _connectionService.GetAsync(context, model.ConnectionId);
            var values = JsonConvert.DeserializeObject<List<CredentialPreviewAttribute>>(model.CredentialAttributes);
            values.Add(new CredentialPreviewAttribute("issuer", issuer.Owner.Name));

            var (offer, _) = await _credentialService.CreateOfferAsync(context, new OfferConfiguration
                {
                    CredentialDefinitionId = model.CredentialDefinitionId,
                    IssuerDid = issuer.IssuerDid,
                    CredentialAttributeValues = values
                });
            await _messageService.SendAsync(context.Wallet, offer, connection);

            return RedirectToAction("Index");
        }

    }
}