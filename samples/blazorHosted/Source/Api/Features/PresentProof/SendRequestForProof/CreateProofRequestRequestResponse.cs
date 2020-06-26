namespace BlazorHosted.Features.PresentProofs
{
  using System;
  using BlazorHosted.Features.Bases;
  using Hyperledger.Aries.Features.PresentProof;

  public class CreateProofRequestRequestResponse : BaseResponse
  {
    public RequestPresentationMessage RequestPresentationMessage { get; set; } = null!;
    public CreateProofRequestRequestResponse() { }

    public CreateProofRequestRequestResponse(RequestPresentationMessage aRequestPresentationMessage, Guid aCorrelationId) : base(aCorrelationId) 
    {
      RequestPresentationMessage = aRequestPresentationMessage;
    }
  }
}
