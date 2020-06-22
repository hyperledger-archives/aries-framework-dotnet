namespace BlazorHosted.Features.Connections.Components
{
  using BlazorHosted.Features.Bases;
  using BlazorHosted.Features.Connections;
  using Hyperledger.Aries.Features.DidExchange;
  using Microsoft.AspNetCore.Components;
  using System.Threading.Tasks;

  public partial class Details : BaseComponent
  {
    public ConnectionRecord Connection =>
      ConnectionState.Connections[ConnectionId];

    [Parameter] public string ConnectionId { get; set; }

    protected override Task OnParametersSetAsync()
    {
      // Should fetch the single item if coming form list will be in mem already but if direct it won't be.
      //_ = await Mediator.Send(new FetchConnectionAction());
      return base.OnParametersSetAsync();
    }
  }
}
