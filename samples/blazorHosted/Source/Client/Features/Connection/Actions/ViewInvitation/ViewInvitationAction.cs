namespace Hyperledger.Aries.OpenApi.Features.Connections
{
  using Hyperledger.Aries.OpenApi.Features.Bases;

  internal partial class ConnectionState
  {
    public class ViewInvitationAction : BaseAction 
    {
      public string InvitationDetails { get; set; }
    }
  }
}
