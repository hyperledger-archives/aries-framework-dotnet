namespace BlazorHosted.Features.PresentProofs
{
  using System;
  using System.Collections.Generic;
  using BlazorHosted.Features.Bases;

  public class GetCredentialsForProofResponse : BaseResponse
  {
    public GetCredentialsForProofResponse() { }

    public GetCredentialsForProofResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
