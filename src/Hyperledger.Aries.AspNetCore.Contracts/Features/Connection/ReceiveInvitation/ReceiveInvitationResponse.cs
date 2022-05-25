namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using Aries.Features.Handshakes.Connection.Models;
  using Bases;
  using System;

  public class ReceiveInvitationResponse : BaseResponse
  {
    public ConnectionInvitationMessage ConnectionInvitationMessage { get; set; } = null!;

    public ReceiveInvitationResponse() { }

    public ReceiveInvitationResponse(Guid aCorrelationId, ConnectionInvitationMessage aConnectionInvitationMessage)
      : base(aCorrelationId) 
    {
      ConnectionInvitationMessage = aConnectionInvitationMessage;
    }
  }
}
