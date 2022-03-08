namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using Aries.Features.Handshakes.Common;
  using Aries.Features.Handshakes.Connection.Models;
  using Bases;
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
