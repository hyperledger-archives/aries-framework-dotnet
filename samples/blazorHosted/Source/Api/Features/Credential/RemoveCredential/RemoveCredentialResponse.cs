namespace BlazorHosted.Features.Credentials
{
  using System;
  using System.Collections.Generic;
  using BlazorHosted.Features.Bases;

  public class RemoveCredentialResponse : BaseResponse
  {
    public RemoveCredentialResponse() { }

    public RemoveCredentialResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
