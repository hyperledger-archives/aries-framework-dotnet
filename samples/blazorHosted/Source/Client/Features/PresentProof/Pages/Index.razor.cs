namespace BlazorHosted.Features.PresentProofs.Pages
{
  using BlazorState.Features.Routing;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  public partial class Index
  {
    public const string RouteTemplate = "/proofs";

    public static string GetRoute() => RouteTemplate;

    protected async Task CreateClick() =>
      _ = await Mediator.Send(new RouteState.ChangeRouteAction { NewRoute = Create.GetRoute() });

    protected override async Task OnInitializedAsync()
    {
      //_ = await Mediator.Send(new FetchProofsAction());

      await base.OnInitializedAsync();
    }
  }
}
