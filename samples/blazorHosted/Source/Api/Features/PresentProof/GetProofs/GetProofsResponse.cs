namespace BlazorHosted.Features.PresentProofs
{
  using System;
  using System.Collections.Generic;
  using BlazorHosted.Features.Bases;

  public class GetProofsResponse : BaseResponse
  {
    public GetProofsResponse() { }

    public GetProofsResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
