namespace BlazorHosted.Features.Connections
{
  using BlazorHosted.Features.Bases;

  internal partial class ConnectionState
  {
    public class ViewInvitationAction : BaseAction 
    {
      public string InvitationDetails { get; set; }
    }
  }
}
