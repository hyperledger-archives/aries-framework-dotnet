namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  internal partial class ConnectionState
  {
    public class DeleteConnectionAction : BaseAction 
    {
      public string ConnectionId { get; set; }

      public DeleteConnectionAction(string aConnectionId)
      {
        ConnectionId = aConnectionId;
      }
    }
  }
}
