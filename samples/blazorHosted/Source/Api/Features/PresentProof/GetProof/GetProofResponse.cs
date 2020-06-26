namespace BlazorHosted.Features.PresentProofs
{
  using System;
  using BlazorHosted.Features.Bases;
  using Hyperledger.Aries.Features.PresentProof;

  public class GetProofResponse : BaseResponse
  {
    public GetProofResponse() { }

    public ProofRecord ProofRecord { get; set; } = null!;

    public GetProofResponse(ProofRecord aProofRecord, Guid aCorrelationId) : base(aCorrelationId) 
    {
      ProofRecord = aProofRecord;
    }
  }
}
