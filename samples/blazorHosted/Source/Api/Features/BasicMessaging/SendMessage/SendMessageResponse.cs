namespace BlazorHosted.Features.BasicMessaging
{
  using BlazorHosted.Features.Bases;
  using System;

  public class SendMessageResponse : BaseResponse
  {
    public SendMessageResponse() { }

    public SendMessageResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
