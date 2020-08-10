namespace Hyperledger.Aries.AspNetCore.Features.PresentProofs
{
  using System;
  using System.Collections.Generic;
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using Hyperledger.Aries.Features.PresentProof;

  public class GetProofsResponse : BaseResponse
  {
    public GetProofsResponse() { }

    public GetProofsResponse(List<ProofRecord> aProofRecords, Guid aCorrelationId) : base(aCorrelationId) 
    {
      ProofRecords = aProofRecords;
    }

    public List<ProofRecord> ProofRecords { get; set; } = null!;
  }
}
