namespace BlazorHosted.Features.Connections
{
  using BlazorHosted.Features.Bases;

  internal partial class ConnectionState
  {
    
    public class FetchConnectionAction : BaseAction 
    {
      public string ConnectionId { get; set; }

      public FetchConnectionAction(string aConnectionId)
      {
        ConnectionId = aConnectionId;
      }
    }
  }
}