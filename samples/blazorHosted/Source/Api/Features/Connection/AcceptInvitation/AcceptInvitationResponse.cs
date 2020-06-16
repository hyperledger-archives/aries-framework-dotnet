namespace BlazorHosted.Features.Connections
{
  using System;
  using System.Collections.Generic;
  using BlazorHosted.Features.Bases;

  public class AcceptInvitationResponse : BaseResponse
  {
    public AcceptInvitationResponse() { }

    public AcceptInvitationResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
