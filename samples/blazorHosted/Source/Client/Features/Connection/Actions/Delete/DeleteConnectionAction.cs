namespace Hyperledger.Aries.OpenApi.Features.Connections
{
  using Hyperledger.Aries.OpenApi.Features.Bases;

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
