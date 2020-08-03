namespace Hyperledger.Aries.AspNetCore.Features.IssueCredentials
{
  using Hyperledger.Aries.Agents;
  using Hyperledger.Aries.Configuration;
  using Hyperledger.Aries.Features.DidExchange;
  using Hyperledger.Aries.Features.IssueCredential;
  using MediatR;
  using System.Threading;
  using System.Threading.Tasks;

  public class OfferCredentialHandler : IRequestHandler<OfferCredentialRequest, OfferCredentialResponse>
  {
    private readonly IAgentProvider AgentProvider;
    private readonly IProvisioningService ProvisioningService;
    private readonly IConnectionService ConnectionService;
    private readonly ICredentialService CredentialService;
    private readonly IMessageService MessageService;

    public OfferCredentialHandler
    (
      IAgentProvider aAgentProvider,
      IProvisioningService aProvisioningService,
      IConnectionService aConnectionService,
      ICredentialService aCredentialService,
      IMessageService aMessageService
    )
    {
      AgentProvider = aAgentProvider;
      ProvisioningService = aProvisioningService;
      ConnectionService = aConnectionService;
      CredentialService = aCredentialService;
      MessageService = aMessageService;
    }

    public async Task<OfferCredentialResponse> Handle
    (
      OfferCredentialRequest aOfferCredentialRequest,
      CancellationToken aCancellationToken
    )
    {
      IAgentContext agentContext = await AgentProvider.GetContextAsync();
      ProvisioningRecord provisioningRecord = await ProvisioningService.GetProvisioningAsync(agentContext.Wallet);
      ConnectionRecord connectionRecord = await ConnectionService.GetAsync(agentContext, aOfferCredentialRequest.ConnectionId);

      (CredentialOfferMessage credentialOfferMessage, _) = 
        await CredentialService.CreateOfferAsync
        (
          agentContext, 
          new OfferConfiguration 
          { 
            CredentialDefinitionId = aOfferCredentialRequest.CredentialDefinitionId,
            IssuerDid = provisioningRecord.IssuerDid,
            CredentialAttributeValues = aOfferCredentialRequest.CredentialPreviewAttributes
          }
        );

      await MessageService.SendAsync(agentContext.Wallet, credentialOfferMessage, connectionRecord);

      var response = new OfferCredentialResponse(aOfferCredentialRequest.CorrelationId);

      return response;
    }
  }
}
