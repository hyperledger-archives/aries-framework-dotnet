namespace BlazorHosted.Features.Connections
{
  using System;
  using BlazorHosted.Features.Bases;
  using Hyperledger.Aries.Features.DidExchange;

  public class RecieveInvitationResponse : BaseResponse
  {
    public ConnectionInvitationMessage ConnectionInvitationMessage { get; set; } = null!;

    public RecieveInvitationResponse() { }

    public RecieveInvitationResponse(Guid aCorrelationId, ConnectionInvitationMessage aConnectionInvitationMessage)
      : base(aCorrelationId) 
    {
      ConnectionInvitationMessage = aConnectionInvitationMessage;
    }
  }
}
