namespace BlazorHosted.Features.Credentials
{
  using System;
  using System.Collections.Generic;
  using BlazorHosted.Features.Bases;

  public class GetCredentialsResponse : BaseResponse
  {
    public GetCredentialsResponse() { }

    public GetCredentialsResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
