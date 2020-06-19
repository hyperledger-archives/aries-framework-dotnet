namespace BlazorHosted.Features.Connections
{
  using BlazorState;
  using Hyperledger.Aries.Features.DidExchange;
  using System.Collections.Generic;

  internal partial class ConnectionState : State<ConnectionState>
  {
    public List<ConnectionRecord> _ConnectionRecords { get; set; } = null!;

    public IReadOnlyList<ConnectionRecord> ConnectionRecords => _ConnectionRecords.AsReadOnly();
    public ConnectionState() 
    {
      _ConnectionRecords = new List<ConnectionRecord>();
    }

    public override void Initialize() { }

  }
}
