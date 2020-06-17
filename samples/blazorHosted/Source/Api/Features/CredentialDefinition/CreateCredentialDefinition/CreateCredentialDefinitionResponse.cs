namespace BlazorHosted.Features.CredentialDefinitions
{
  using System;
  using System.Collections.Generic;
  using BlazorHosted.Features.Bases;

  public class CreateCredentialDefinitionResponse : BaseResponse
  {
    public CreateCredentialDefinitionResponse() { }

    public CreateCredentialDefinitionResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
