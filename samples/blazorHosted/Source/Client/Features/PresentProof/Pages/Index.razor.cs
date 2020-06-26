namespace BlazorHosted.Features.PresentProofs.Pages
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  public partial class Index
  {
    public const string RouteTemplate = "/proofs";

    public static string GetRoute() => RouteTemplate;

    protected override async Task OnInitializedAsync()
    {
      //_ = await Mediator.Send(new FetchProofsAction());

      await base.OnInitializedAsync();
    }
  }
}
