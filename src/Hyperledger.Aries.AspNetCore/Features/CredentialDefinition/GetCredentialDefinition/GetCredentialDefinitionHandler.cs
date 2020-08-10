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
  
  public class GetCredentialDefinitionHandler : IRequestHandler<GetCredentialDefinitionRequest, GetCredentialDefinitionResponse>
  {

    private readonly IAgentProvider AgentProvider;
    private readonly ISchemaService SchemaService;

    public GetCredentialDefinitionHandler(IAgentProvider aAgentProvider, ISchemaService aSchemaService)
    {
      AgentProvider = aAgentProvider;
      SchemaService = aSchemaService;
    }

    public async Task<GetCredentialDefinitionResponse> Handle
    (
      GetCredentialDefinitionRequest aGetCredentialDefinitionRequest,
      CancellationToken aCancellationToken
    )
    {
      IAgentContext agentContext = await AgentProvider.GetContextAsync();

      List<DefinitionRecord> definitionRecords =
        await SchemaService.ListCredentialDefinitionsAsync(agentContext.Wallet);

      DefinitionRecord definitionRecord = 
        definitionRecords
        .FirstOrDefault
        (
          aDefinitionRecord => aDefinitionRecord.Id == aGetCredentialDefinitionRequest.CredentialDefinitionId
        );

      var response = 
        new GetCredentialDefinitionResponse(aGetCredentialDefinitionRequest.CorrelationId, definitionRecord);

      return response;
    }
  }
}
