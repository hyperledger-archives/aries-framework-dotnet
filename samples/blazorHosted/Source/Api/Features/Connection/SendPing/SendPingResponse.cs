namespace Hyperledger.Aries.OpenApi.Features.Connections
{
  using System;
  using System.Collections.Generic;
  using Hyperledger.Aries.OpenApi.Features.Bases;

  public class SendPingResponse : BaseResponse
  {
    public bool Success { get; set; }

    public SendPingResponse() { }

    public SendPingResponse(Guid aCorrelationId, bool aSuccess) : base(aCorrelationId) { Success = aSuccess; }
  }
}
