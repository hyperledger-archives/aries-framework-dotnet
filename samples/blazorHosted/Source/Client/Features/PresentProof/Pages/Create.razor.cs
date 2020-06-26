namespace BlazorHosted.Features.PresentProofs.Pages
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using static BlazorHosted.Features.PresentProof.Actions.CreateAndSend.PresentProofState;
  using static BlazorState.Features.Routing.RouteState;

  public partial class Create
  {
    public const string RouteTemplate = "/proofs/create";

    public static string GetRoute() => RouteTemplate;

    public SendRequestForProofRequest SendRequestForProofRequest { get; set; }
    protected void CreateClick()
    {
      //var createCredentialDefinitionAction = new CreateAndSendProofRequestAction()
      //{
      //  CreateCredentialDefinitionRequest = new SendRequestForProofRequest
      //  {
      //    Comment = "Some Comment",
          
      //  }
      //};
      //_ = await Mediator.Send(createCredentialDefinitionAction);
      //_ = await Mediator.Send(new ChangeRouteAction { NewRoute = Pages.Index.RouteTemplate });
    }

    protected async Task HandleValidSubmit()
    {
      _ = await Mediator.Send(new CreateAndSendProofRequestAction { SendRequestForProofRequest = SendRequestForProofRequest });
      _ = await Mediator.Send(new ChangeRouteAction { NewRoute = Pages.Index.RouteTemplate });
    }

    protected override async Task OnInitializedAsync()
    {
      SendRequestForProofRequest = new SendRequestForProofRequest();

      //_ = await Mediator.Send(new FetchSchemasAction());
      await base.OnInitializedAsync();
    }

  }
}
