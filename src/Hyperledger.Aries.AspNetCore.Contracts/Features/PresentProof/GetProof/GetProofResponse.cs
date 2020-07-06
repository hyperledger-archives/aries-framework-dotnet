namespace Hyperledger.Aries.AspNetCore.Features.PresentProofs
{
  using System;
  using Hyperledger.Aries.AspNetCore.Features.Bases;
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
