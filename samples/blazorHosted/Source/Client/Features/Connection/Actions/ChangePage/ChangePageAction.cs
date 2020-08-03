namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  internal partial class ConnectionState
  {
    public class ChangePageAction : BaseAction 
    {
      public int  PageIndex { get; set; }
    }
  }
}
