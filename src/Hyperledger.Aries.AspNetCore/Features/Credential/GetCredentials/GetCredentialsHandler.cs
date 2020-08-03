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
  
  public class GetCredentialsHandler : IRequestHandler<GetCredentialsRequest, GetCredentialsResponse>
  {
    private readonly IAgentProvider AgentProvider;
    private readonly ICredentialService CredentialService;

    public GetCredentialsHandler(IAgentProvider aAgentProvider, ICredentialService aCredentialService)
    {
      AgentProvider = aAgentProvider;
      CredentialService = aCredentialService;
    }

    public async Task<GetCredentialsResponse> Handle
    (
      GetCredentialsRequest aGetCredentialsRequest,
      CancellationToken aCancellationToken
    )
    {
      IAgentContext agentContext = await AgentProvider.GetContextAsync();
      List<CredentialRecord> credentialRecords = await CredentialService.ListAsync(agentContext);

      var response = new GetCredentialsResponse(aGetCredentialsRequest.CorrelationId, credentialRecords);

      return await Task.Run(() => response);
    }
  }
}
