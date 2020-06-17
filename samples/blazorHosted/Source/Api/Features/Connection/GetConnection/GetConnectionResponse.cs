namespace BlazorHosted.Features.Connections
{
  using System;
  using System.Collections.Generic;
  using BlazorHosted.Features.Bases;
  using Hyperledger.Aries.Features.DidExchange;

  public class GetConnectionResponse : BaseResponse
  {
    public ConnectionRecord ConnectionRecord { get; set; } = null!;
    public GetConnectionResponse() { }

    public GetConnectionResponse(Guid aCorrelationId, ConnectionRecord aConnectionRecord) : base(aCorrelationId) 
    {
      ConnectionRecord = aConnectionRecord;
    }
  }
}
