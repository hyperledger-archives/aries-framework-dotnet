namespace Hyperledger.Aries.OpenApi.Features.PresentProofs
{
  using System;
  using System.Collections.Generic;
  using Hyperledger.Aries.OpenApi.Features.Bases;

  public class AcceptProofRequestResponse : BaseResponse
  {
    public AcceptProofRequestResponse() { }

    public AcceptProofRequestResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
