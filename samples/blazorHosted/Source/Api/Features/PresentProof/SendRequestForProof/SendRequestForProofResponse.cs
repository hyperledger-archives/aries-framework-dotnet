namespace BlazorHosted.Features.PresentProofs
{
  using System;
  using System.Collections.Generic;
  using BlazorHosted.Features.Bases;

  public class SendRequestForProofResponse : BaseResponse
  {
    public SendRequestForProofResponse() { }

    public SendRequestForProofResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
