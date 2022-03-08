namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using Aries.Features.Handshakes.Common;
  using Bases;
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
