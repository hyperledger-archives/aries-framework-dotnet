namespace Hyperledger.Aries.OpenApi.Features.Connections
{
  using Hyperledger.Aries.OpenApi.Features.Bases;
  using Hyperledger.Aries.Features.DidExchange;
  using System;

  public class GetConnectionResponse : BaseResponse
  {
    public ConnectionRecord? ConnectionRecord { get; set; }

    public GetConnectionResponse() { }

    public GetConnectionResponse(Guid aCorrelationId, ConnectionRecord aConnectionRecord) : base(aCorrelationId)
    {
      ConnectionRecord = aConnectionRecord;
    }
  }
}
