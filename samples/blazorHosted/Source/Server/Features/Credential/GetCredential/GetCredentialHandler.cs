namespace BlazorHosted.Features.Credentials
{
  using Hyperledger.Aries.Agents;
  using Hyperledger.Aries.Features.IssueCredential;
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  
  public class GetCredentialHandler : IRequestHandler<GetCredentialRequest, GetCredentialResponse>
  {
    private readonly IAgentProvider AgentProvider;
    private readonly ICredentialService CredentialService;

    public GetCredentialHandler(IAgentProvider aAgentProvider, ICredentialService aCredentialService)
    {
      AgentProvider = aAgentProvider;
      CredentialService = aCredentialService;
    }

    public async Task<GetCredentialResponse> Handle
    (
      GetCredentialRequest aGetCredentialRequest,
      CancellationToken aCancellationToken
    )
    {

      IAgentContext agentContext = await AgentProvider.GetContextAsync();
      List<CredentialRecord> credentialRecords = await CredentialService.ListAsync(agentContext);

      CredentialRecord credentialRecord =
        credentialRecords
          .FirstOrDefault(aCredentialRecord => aCredentialRecord.CredentialId == aGetCredentialRequest.CredentialId);

      var response = new GetCredentialResponse(aGetCredentialRequest.CorrelationId, credentialRecord);

      return await Task.Run(() => response);
    }
  }
}
