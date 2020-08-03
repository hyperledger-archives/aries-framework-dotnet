namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;

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