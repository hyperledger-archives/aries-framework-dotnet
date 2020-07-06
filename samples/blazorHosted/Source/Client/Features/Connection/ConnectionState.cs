namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using BlazorState;
  using Hyperledger.Aries.Features.DidExchange;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.Json.Serialization;

  internal partial class ConnectionState : State<ConnectionState>
  {
    public int PageSize { get; private set; }
    public int PageIndex { get; private set; }

    private Dictionary<string, ConnectionRecord> _ConnectionRecords;

    public string InvitationUrl { get; private set; }
    public string InvitationQrUri { get; private set; }

    public ConnectionInvitationMessage ConnectionInvitationMessage { get; private set; }

    [JsonIgnore]
    public IReadOnlyDictionary<string, ConnectionRecord> Connections => _ConnectionRecords;

    public IReadOnlyList<ConnectionRecord> ConnectionsAsList => _ConnectionRecords.Values.ToList();

    public ConnectionState() { }

    /// <summary>
    /// Set the Initial State
    /// </summary>
    public override void Initialize()
    {
      PageIndex = 0;
      PageSize = 5;
      _ConnectionRecords = new Dictionary<string, ConnectionRecord>();
      InvitationUrl = null;
      InvitationQrUri = null;
    }
  }
}
