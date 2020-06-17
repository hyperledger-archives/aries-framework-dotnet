namespace BlazorHosted.Features.Credentials
{
  using System;
  using System.Collections.Generic;
  using BlazorHosted.Features.Bases;

  public class GetCredentialResponse : BaseResponse
  {
    public GetCredentialResponse() { }

    public GetCredentialResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
