namespace BlazorHosted.Features.CredentialDefinitions
{
  using BlazorHosted.Features.Bases;
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
