namespace Hyperledger.Aries.AspNetCore.Features.Connections.Components
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using Hyperledger.Aries.AspNetCore.Features.Connections;
  using Hyperledger.Aries.Features.DidExchange;
  using Microsoft.AspNetCore.Components;
  using System.Threading.Tasks;
  using static Hyperledger.Aries.AspNetCore.Features.Connections.ConnectionState;

  public partial class Details : BaseComponent
  {
    public ConnectionRecord Connection
    {
      get
      {
        ConnectionState.Connections.TryGetValue(ConnectionId, out ConnectionRecord value);
        return value;
      }
    }

    [Parameter] public string ConnectionId { get; set; }

    protected override async Task OnParametersSetAsync()
    {
      _ = await Mediator.Send(new FetchConnectionAction(ConnectionId));
      await base.OnParametersSetAsync();
    }
  }
}
