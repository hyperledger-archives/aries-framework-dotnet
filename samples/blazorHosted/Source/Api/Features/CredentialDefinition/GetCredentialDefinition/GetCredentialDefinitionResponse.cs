namespace BlazorHosted.Features.CredentialDefinitions
{
  using System;
  using System.Collections.Generic;
  using BlazorHosted.Features.Bases;

  public class GetCredentialDefinitionResponse : BaseResponse
  {
    public GetCredentialDefinitionResponse() { }

    public GetCredentialDefinitionResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
