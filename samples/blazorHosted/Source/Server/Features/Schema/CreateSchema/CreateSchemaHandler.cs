namespace BlazorHosted.Features.Schemas
{
  using Hyperledger.Aries.Agents;
  using Hyperledger.Aries.Configuration;
  using Hyperledger.Aries.Extensions;
  using Hyperledger.Aries.Features.IssueCredential;
  using Hyperledger.Aries.Models.Records;
  using Hyperledger.Indy.DidApi;
  using Hyperledger.Indy.LedgerApi;
  using MediatR;
  using System.Threading;
  using System.Threading.Tasks;

  public class CreateSchemaHandler : IRequestHandler<CreateSchemaRequest, CreateSchemaResponse>
  {
    private readonly IAgentProvider AgentProvider;
    private readonly IProvisioningService ProvisioningService;
    private readonly ISchemaService SchemaService;

    public CreateSchemaHandler
    (
      IAgentProvider aAgentProvider,
      IProvisioningService aProvisioningService,
      ISchemaService aSchemaService
    )
    {
      AgentProvider = aAgentProvider;
      ProvisioningService = aProvisioningService;
      SchemaService = aSchemaService;
    }

    public async Task<CreateSchemaResponse> Handle
    (
      CreateSchemaRequest aCreateSchemaRequest,
      CancellationToken aCancellationToken
    )
    {
      IAgentContext agentContext = await AgentProvider.GetContextAsync();
      ProvisioningRecord issuerProvisioningRecord = await ProvisioningService.GetProvisioningAsync(agentContext.Wallet);
      
      CreateAndStoreMyDidResult trusteeCreateAndStoreMyDidResult =
        await Did.CreateAndStoreMyDidAsync
        (
          agentContext.Wallet,
          new { seed = "000000000000000000000000Trustee1" }.ToJson()
        );

      await Ledger.SignAndSubmitRequestAsync
      (
        await agentContext.Pool,
        agentContext.Wallet,
        trusteeCreateAndStoreMyDidResult.Did,
        requestJson: await Ledger.BuildNymRequestAsync
        (
          trusteeCreateAndStoreMyDidResult.Did, 
          issuerProvisioningRecord.IssuerDid, 
          issuerProvisioningRecord.IssuerVerkey,
          alias: null,
          role: "ENDORSER"
        )
      );

      string schemaId =
        await SchemaService.CreateSchemaAsync
        (
          agentContext,
          issuerDid: issuerProvisioningRecord.IssuerDid,
          aCreateSchemaRequest.Name,
          aCreateSchemaRequest.Version,
          aCreateSchemaRequest.AttributeNames.ToArray()
        );

      var response = new CreateSchemaResponse(aCreateSchemaRequest.CorrelationId, schemaId);

      return response;
    }
  }
}
