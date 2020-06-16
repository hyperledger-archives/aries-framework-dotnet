namespace BlazorHosted.Features.Connections
{
  using System;
  using System.Collections.Generic;
  using BlazorHosted.Features.Bases;

  public class RecieveInvitationResponse : BaseResponse
  {
    public RecieveInvitationResponse() { }

    public RecieveInvitationResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
