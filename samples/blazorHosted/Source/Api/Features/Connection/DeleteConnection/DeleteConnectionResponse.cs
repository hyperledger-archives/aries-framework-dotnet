namespace Hyperledger.Aries.OpenApi.Features.Connections
{
  using Hyperledger.Aries.OpenApi.Features.Bases;
  using System;

  public class DeleteConnectionResponse : BaseResponse
  {
    public DeleteConnectionResponse() { }

    public DeleteConnectionResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
