namespace Hyperledger.Aries.AspNetCore.Features.Schemas
{
  using Hyperledger.Aries.Agents;
  using Hyperledger.Aries.Features.IssueCredential;
  using Hyperledger.Aries.Models.Records;
  using MediatR;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;

  public class GetSchemaHandler : IRequestHandler<GetSchemaRequest, GetSchemaResponse>
  {
    private readonly IAgentProvider AgentProvider;
    private readonly ISchemaService SchemaService;

    public GetSchemaHandler(IAgentProvider aAgentProvider, ISchemaService aSchemaService)
    {
      AgentProvider = aAgentProvider;
      SchemaService = aSchemaService;
    }

    public async Task<GetSchemaResponse> Handle
    (
      GetSchemaRequest aGetSchemaRequest,
      CancellationToken aCancellationToken
    )
    {
      IAgentContext agentContext = await AgentProvider.GetContextAsync();
      List<SchemaRecord> schemaRecords = await SchemaService.ListSchemasAsync(agentContext.Wallet);
      SchemaRecord schemaRecord = schemaRecords.FirstOrDefault(aSchemaRecord => aSchemaRecord.Id == aGetSchemaRequest.SchemaId);

      var response = new GetSchemaResponse(aGetSchemaRequest.CorrelationId, schemaRecord);

      return await Task.Run(() => response);
    }
  }
}
