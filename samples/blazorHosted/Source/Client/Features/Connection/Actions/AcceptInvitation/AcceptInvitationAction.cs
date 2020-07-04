namespace Hyperledger.Aries.OpenApi.Features.Connections
{
  using Hyperledger.Aries.OpenApi.Features.Bases;

  internal partial class ConnectionState
  {
    public class AcceptInvitationAction : BaseAction 
    {
      public string InvitationDetails { get; set; }
    }
  }
}
