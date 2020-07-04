namespace Hyperledger.Aries.OpenApi.Features.CredentialDefinitions
{
  using Hyperledger.Aries.OpenApi.Features.Bases;
  using System;

  public class CreateCredentialDefinitionResponse : BaseResponse
  {
    public string CredentialDefinitionId { get; set; } = null!;

    public CreateCredentialDefinitionResponse() { }

    public CreateCredentialDefinitionResponse(Guid aCorrelationId, string aCredentialDefinitionId) : base(aCorrelationId)
    {
      CredentialDefinitionId = aCredentialDefinitionId;
    }
  }
}
