namespace Hyperledger.Aries.AspNetCore.Features.Schemas
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
