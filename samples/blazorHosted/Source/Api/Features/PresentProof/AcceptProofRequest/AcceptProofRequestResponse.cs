namespace BlazorHosted.Features.PresentProofs
{
  using System;
  using System.Collections.Generic;
  using BlazorHosted.Features.Bases;

  public class AcceptProofRequestResponse : BaseResponse
  {
    public AcceptProofRequestResponse() { }

    public AcceptProofRequestResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
