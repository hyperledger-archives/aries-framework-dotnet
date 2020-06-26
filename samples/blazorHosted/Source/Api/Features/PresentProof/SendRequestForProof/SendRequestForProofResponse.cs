namespace BlazorHosted.Features.PresentProofs
{
  using System;
  using BlazorHosted.Features.Bases;
  using Hyperledger.Aries.Features.PresentProof;

  public class SendRequestForProofResponse : BaseResponse
  {
    public RequestPresentationMessage RequestPresentationMessage { get; set; } = null!;
    public SendRequestForProofResponse() { }

    public SendRequestForProofResponse(RequestPresentationMessage aRequestPresentationMessage, Guid aCorrelationId) : base(aCorrelationId) 
    {
      RequestPresentationMessage = aRequestPresentationMessage;
    }
  }
}
