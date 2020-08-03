namespace Hyperledger.Aries.AspNetCore.Features.CredentialDefinitions
{
  using Hyperledger.Aries.Agents;
  using Hyperledger.Aries.Configuration;
  using Hyperledger.Aries.Features.IssueCredential;
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;

  public class CreateCredentialDefinitionHandler : IRequestHandler<CreateCredentialDefinitionRequest, CreateCredentialDefinitionResponse>
  {
    private readonly IAgentProvider AgentProvider;
    private readonly IProvisioningService ProvisioningService;
    private readonly ISchemaService SchemaService;

    public CreateCredentialDefinitionHandler
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

    public async Task<CreateCredentialDefinitionResponse> Handle
    (
      CreateCredentialDefinitionRequest aCreateCredentialDefinitionRequest,
      CancellationToken aCancellationToken
    )
    {
      IAgentContext agentContext = await AgentProvider.GetContextAsync();

      ProvisioningRecord issuerProvisioningRecord = await ProvisioningService.GetProvisioningAsync(agentContext.Wallet);
      string credentialDefinitionId =
        await SchemaService.CreateCredentialDefinitionAsync
        (
          agentContext,
          new CredentialDefinitionConfiguration
          {
            SchemaId = aCreateCredentialDefinitionRequest.SchemaId,
            Tag = aCreateCredentialDefinitionRequest.Tag,
            EnableRevocation = aCreateCredentialDefinitionRequest.EnableRevocation,
            RevocationRegistrySize = aCreateCredentialDefinitionRequest.RevocationRegistrySize,
            RevocationRegistryBaseUri = aCreateCredentialDefinitionRequest.RevocationRegistryBaseUri?.ToString(),
            RevocationRegistryAutoScale = aCreateCredentialDefinitionRequest.RevocationRegistryAutoScale,
            IssuerDid = issuerProvisioningRecord.IssuerDid
          }
        );

      var response = new CreateCredentialDefinitionResponse(aCreateCredentialDefinitionRequest.CorrelationId, credentialDefinitionId);

      return response;
    }
  }
}
