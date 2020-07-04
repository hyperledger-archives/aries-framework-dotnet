namespace Hyperledger.Aries.OpenApi.Features.Connections
{
  using System;
  using System.Collections.Generic;
  using Hyperledger.Aries.OpenApi.Features.Bases;

  public class AcceptInvitationResponse : BaseResponse
  {
    public AcceptInvitationResponse() { }

    public AcceptInvitationResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
