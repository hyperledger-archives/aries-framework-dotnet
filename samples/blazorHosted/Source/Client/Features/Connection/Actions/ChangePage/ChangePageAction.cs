namespace Hyperledger.Aries.OpenApi.Features.Connections
{
  using Hyperledger.Aries.OpenApi.Features.Bases;

  internal partial class ConnectionState
  {
    public class ChangePageAction : BaseAction 
    {
      public int  PageIndex { get; set; }
    }
  }
}
