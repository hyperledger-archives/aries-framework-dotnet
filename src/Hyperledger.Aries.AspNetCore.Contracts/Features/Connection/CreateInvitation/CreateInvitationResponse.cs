namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using Hyperledger.Aries.Features.DidExchange;
  using System;

  public class CreateInvitationResponse : BaseResponse
  {
    /// <summary>
    /// Represents an invitation message for establishing connection.
    /// </summary>
    public ConnectionInvitationMessage ConnectionInvitationMessage { get; set; } = null!;

    public ConnectionRecord ConnectionRecord { get; set; } = null!;

    public string InvitationUrl { get; set; } = null!;

    public CreateInvitationResponse() { }

    public CreateInvitationResponse
    (
      Guid aRequestId,
      ConnectionInvitationMessage aConnectionInvitationMessage,
      ConnectionRecord aConnectionRecord,
      string aInvitationUrl
    ) : base(aRequestId)
    {
      ConnectionInvitationMessage = aConnectionInvitationMessage;
      ConnectionRecord = aConnectionRecord;
      InvitationUrl = aInvitationUrl;
    }
  }
}
