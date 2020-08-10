namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  internal partial class ConnectionState
  {
    public class ViewInvitationAction : BaseAction 
    {
      public string InvitationDetails { get; set; }
    }
  }
}
