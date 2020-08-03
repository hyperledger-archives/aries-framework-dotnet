namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using System;
  using System.Collections.Generic;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public class AcceptInvitationResponse : BaseResponse
  {
    public AcceptInvitationResponse() { }

    public AcceptInvitationResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
