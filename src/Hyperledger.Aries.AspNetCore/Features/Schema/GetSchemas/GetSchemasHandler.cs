namespace Hyperledger.Aries.AspNetCore.Features.Schemas
{
  using Hyperledger.Aries.Agents;
  using Hyperledger.Aries.Features.IssueCredential;
  using Hyperledger.Aries.Models.Records;
  using MediatR;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;

  public class GetSchemasHandler : IRequestHandler<GetSchemasRequest, GetSchemasResponse>
  {
    private readonly IAgentProvider AgentProvider;
    private readonly ISchemaService SchemaService;

    public GetSchemasHandler(IAgentProvider aAgentProvider, ISchemaService aSchemaService)
    {
      AgentProvider = aAgentProvider;
      SchemaService = aSchemaService;
    }

    public async Task<GetSchemasResponse> Handle
    (
      GetSchemasRequest aGetSchemasRequest,
      CancellationToken aCancellationToken
    )
    {
      IAgentContext agentContext = await AgentProvider.GetContextAsync();
      List<SchemaRecord> schemaRecords = await SchemaService.ListSchemasAsync(agentContext.Wallet);
      var response = new GetSchemasResponse(aGetSchemasRequest.CorrelationId, schemaRecords);

      return response;
    }
  }
}
