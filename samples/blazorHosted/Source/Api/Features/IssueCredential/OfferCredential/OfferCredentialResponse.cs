namespace BlazorHosted.Features.IssueCredentials
{
  using System;
  using System.Collections.Generic;
  using BlazorHosted.Features.Bases;

  public class OfferCredentialResponse : BaseResponse
  {
    public OfferCredentialResponse() { }

    public OfferCredentialResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
