namespace BlazorHosted.Features.PresentProofs
{
  using System;
  using System.Collections.Generic;
  using BlazorHosted.Features.Bases;

  public class GetProofResponse : BaseResponse
  {
    public GetProofResponse() { }

    public GetProofResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
