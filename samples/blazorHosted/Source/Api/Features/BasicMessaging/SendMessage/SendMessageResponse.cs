namespace Hyperledger.Aries.OpenApi.Features.BasicMessaging
{
  using Hyperledger.Aries.OpenApi.Features.Bases;
  using System;

  public class SendMessageResponse : BaseResponse
  {
    public SendMessageResponse() { }

    public SendMessageResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
