namespace BlazorHosted.Features.Connections
{
  using BlazorHosted.Features.Bases;

  internal partial class ConnectionState
  {
    public class DeleteConnectionAction : BaseAction 
    {
      public string ConnectionId { get; set; }
    }
  }
}
