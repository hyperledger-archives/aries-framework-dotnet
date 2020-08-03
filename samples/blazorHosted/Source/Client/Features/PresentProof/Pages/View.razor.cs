namespace Hyperledger.Aries.AspNetCore.Features.PresentProofs.Pages
{
  using Hyperledger.Aries.Features.PresentProof;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using static Hyperledger.Aries.AspNetCore.Features.PresentProofs.PresentProofState;
  using static BlazorState.Features.Routing.RouteState;

  public partial class View
  {
    public const string RouteTemplate = "/proofs/view";

    public static string GetRoute() => RouteTemplate;

    public string EncodedProofRequestMessage { get; set; }

    protected async Task OnViewClickAsync() => _ = await Mediator.Send(new ViewProofRequestAction { EncodedProofRequestMessage = EncodedProofRequestMessage });

    public RequestPresentationMessage RequestPresentationMessage => PresentProofState.RequestPresentationMessage;

    protected async Task OnAcceptClickAsync()
    {
      _ = await Mediator.Send(new AcceptProofRequestAction { EncodedProofRequestMessage = EncodedProofRequestMessage });
      _ = await Mediator.Send(new ChangeRouteAction { NewRoute = Index.GetRoute() });
    }
  }
}
