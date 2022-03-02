namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using Aries.Features.Handshakes.Common;
  using Bases;
  using System;
  using System.Collections.Generic;

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
