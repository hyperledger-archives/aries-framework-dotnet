namespace BlazorHosted.Features.Connections
{
  using System;
  using System.Collections.Generic;
  using BlazorHosted.Features.Bases;

  public class SendPingResponse : BaseResponse
  {
    public SendPingResponse() { }

    public SendPingResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
