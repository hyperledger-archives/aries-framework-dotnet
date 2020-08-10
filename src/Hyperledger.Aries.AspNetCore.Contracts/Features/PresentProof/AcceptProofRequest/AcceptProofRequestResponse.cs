namespace Hyperledger.Aries.AspNetCore.Features.PresentProofs
{
  using System;
  using System.Collections.Generic;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public class AcceptProofRequestResponse : BaseResponse
  {
    public AcceptProofRequestResponse() { }

    public AcceptProofRequestResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
