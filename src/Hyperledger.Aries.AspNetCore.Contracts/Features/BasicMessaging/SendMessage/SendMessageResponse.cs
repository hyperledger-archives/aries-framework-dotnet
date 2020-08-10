namespace Hyperledger.Aries.AspNetCore.Features.BasicMessaging
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using System;

  public class SendMessageResponse : BaseResponse
  {
    public SendMessageResponse() { }

    public SendMessageResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
