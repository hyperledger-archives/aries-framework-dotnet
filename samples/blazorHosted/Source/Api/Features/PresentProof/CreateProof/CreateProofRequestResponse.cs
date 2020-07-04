namespace Hyperledger.Aries.OpenApi.Features.PresentProofs
{
  using System;
  using Hyperledger.Aries.OpenApi.Features.Bases;
  using Hyperledger.Aries.Features.PresentProof;

  public class CreateProofRequestResponse : BaseResponse
  {
    public RequestPresentationMessage RequestPresentationMessage { get; set; } = null!;

    public CreateProofRequestResponse() { }

    public CreateProofRequestResponse
    (
      RequestPresentationMessage aRequestPresentationMessage,
      Guid aCorrelationId
    ) : base(aCorrelationId)
    {
      RequestPresentationMessage = aRequestPresentationMessage;
    }
  }
}
