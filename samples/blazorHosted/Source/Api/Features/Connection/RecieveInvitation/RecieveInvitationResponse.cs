namespace Hyperledger.Aries.OpenApi.Features.Connections
{
  using System;
  using Hyperledger.Aries.OpenApi.Features.Bases;
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
