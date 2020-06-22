namespace BlazorHosted.Features.Connections
{
  using BlazorHosted.Features.Bases;

  internal partial class ConnectionState
  {
    public class ChangePageAction : BaseAction 
    {
      public int  PageIndex { get; set; }
    }
  }
}
