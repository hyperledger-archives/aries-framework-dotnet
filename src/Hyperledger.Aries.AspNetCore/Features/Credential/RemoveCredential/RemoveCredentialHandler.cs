namespace Hyperledger.Aries.AspNetCore.Features.Credentials
{
  using Hyperledger.Aries.Agents;
  using Hyperledger.Aries.Features.IssueCredential;
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  
  public class RemoveCredentialHandler : IRequestHandler<RemoveCredentialRequest, RemoveCredentialResponse>
  {
    private readonly IAgentProvider AgentProvider;
    private readonly ICredentialService CredentialService;

    public RemoveCredentialHandler(IAgentProvider aAgentProvider, ICredentialService aCredentialService)
    {
      AgentProvider = aAgentProvider;
      CredentialService = aCredentialService;
    }

    public async Task<RemoveCredentialResponse> Handle
    (
      RemoveCredentialRequest aRemoveCredentialRequest,
      CancellationToken aCancellationToken
    )
    {
      IAgentContext agentContext = await AgentProvider.GetContextAsync();
      await CredentialService.DeleteCredentialAsync(agentContext,aRemoveCredentialRequest.CredentialId);
      var response = new RemoveCredentialResponse(aRemoveCredentialRequest.CorrelationId);

      return response;
    }
  }
}
