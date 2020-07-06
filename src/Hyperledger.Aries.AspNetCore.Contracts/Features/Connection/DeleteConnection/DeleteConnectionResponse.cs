namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using System;

  public class DeleteConnectionResponse : BaseResponse
  {
    public DeleteConnectionResponse() { }

    public DeleteConnectionResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
