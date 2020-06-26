namespace BlazorHosted.Features.PresentProofs
{
  using System;
  using BlazorHosted.Features.Bases;
  using Hyperledger.Aries.Features.PresentProof;

  public class CreateProofRequestResponse : BaseResponse
  {
    public RequestPresentationMessage RequestPresentationMessage { get; set; } = null!;

    public string ProofRequestUrl { get; set; } = null!;

    public CreateProofRequestResponse() { }

    public CreateProofRequestResponse
    (
      RequestPresentationMessage aRequestPresentationMessage,
      string aProofRequestUrl,
      Guid aCorrelationId
    ) : base(aCorrelationId) 
    {
      RequestPresentationMessage = aRequestPresentationMessage;
      ProofRequestUrl = aProofRequestUrl;
    }
  }
}
