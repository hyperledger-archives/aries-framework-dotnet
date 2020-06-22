namespace BlazorHosted.Features.Connections.Components
{
  using BlazorHosted.Features.Bases;
  using Microsoft.AspNetCore.Components;
  using System.Threading.Tasks;
  using static BlazorHosted.Features.Connections.ConnectionState;
  using static BlazorState.Features.Routing.RouteState;

  public partial class Create: BaseComponent
  {

    [Parameter] public string RedirectRoute { get; set; }


  }
}
