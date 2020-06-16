namespace BlazorHosted.Features.Connections
{
  using System;
  using System.Collections.Generic;
  using BlazorHosted.Features.Bases;

  public class SendMessageResponse : BaseResponse
  {
    public SendMessageResponse() { }

    public SendMessageResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
