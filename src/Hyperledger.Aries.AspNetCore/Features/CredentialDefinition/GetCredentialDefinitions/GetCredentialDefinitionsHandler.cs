namespace Hyperledger.Aries.AspNetCore.Features.CredentialDefinitions
{
  using Hyperledger.Aries.Agents;
  using Hyperledger.Aries.Features.IssueCredential;
  using Hyperledger.Aries.Models.Records;
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  
  public class GetCredentialDefinitionsHandler : 
    IRequestHandler<GetCredentialDefinitionsRequest, GetCredentialDefinitionsResponse>
  {
    private readonly IAgentProvider AgentProvider;
    private readonly ISchemaService SchemaService;

    public GetCredentialDefinitionsHandler(IAgentProvider aAgentProvider, ISchemaService aSchemaService)
    {
      AgentProvider = aAgentProvider;
      SchemaService = aSchemaService;
    }
    public async Task<GetCredentialDefinitionsResponse> Handle
    (
      GetCredentialDefinitionsRequest aGetCredentialDefinitionsRequest,
      CancellationToken aCancellationToken
    )
    {
      IAgentContext agentContext = await AgentProvider.GetContextAsync();

      List<DefinitionRecord> definitionRecords = 
        await SchemaService.ListCredentialDefinitionsAsync(agentContext.Wallet);

      var response = 
        new GetCredentialDefinitionsResponse(aGetCredentialDefinitionsRequest.CorrelationId, definitionRecords);

      return response;
    }
  }
}
