namespace BlazorHosted.Features.CredentialDefinitions
{
  using System;
  using System.Collections.Generic;
  using BlazorHosted.Features.Bases;

  public class GetCredentialDefinitionsResponse : BaseResponse
  {
    public GetCredentialDefinitionsResponse() { }

    public GetCredentialDefinitionsResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
