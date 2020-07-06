namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using System;
  using System.Collections.Generic;
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using Hyperledger.Aries.Features.DidExchange;

  public class GetConnectionsResponse : BaseResponse
  {
    public List<ConnectionRecord> ConnectionRecords { get; set; } = null!;

    public GetConnectionsResponse() { }

    public GetConnectionsResponse(Guid aCorrelationId, List<ConnectionRecord> aConnectionRecords) 
      : base(aCorrelationId)
    {
      ConnectionRecords = aConnectionRecords;
    }
  }
}
