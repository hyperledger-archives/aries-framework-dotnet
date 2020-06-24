namespace BlazorHosted.Features.Credentials.Pages
{
  using BlazorHosted.Features.Bases;
  using BlazorHosted.Features.IssueCredentials;
  using Hyperledger.Aries.Features.DidExchange;
  using Hyperledger.Aries.Features.IssueCredential;
  using Hyperledger.Aries.Models.Records;
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using System.Linq;
  using static BlazorHosted.Features.Connections.ConnectionState;
  using static BlazorHosted.Features.CredentialDefinitions.CredentialDefinitionState;
  using static BlazorHosted.Features.Credentials.CredentialState;
  using static BlazorHosted.Features.Schemas.SchemaState;
  using static BlazorState.Features.Routing.RouteState;

  public partial class Edit : BaseComponent
  {
    public const string RouteTemplate = "/credentials/offer";

    private IReadOnlyList<SchemaRecord> Schemas => SchemaState.SchemasAsList;
    private IReadOnlyList<ConnectionRecord> Connections => ConnectionState.ConnectionsAsList;

    private IReadOnlyList<DefinitionRecord> CredentialDefintions => CredentialDefinitionState.CredentialDefinitionsAsList;

    public OfferCredentialRequest OfferCredentialRequest { get; set; }

    public static string GetRoute() => RouteTemplate;

    protected void OnCredentialDefintionSelect()
    {
      OfferCredentialRequest.CredentialPreviewAttributes.Clear();
      if (!string.IsNullOrEmpty(OfferCredentialRequest.CredentialDefinitionId))
      {
        string schemaId = CredentialDefinitionState.CredentialDefinitions[OfferCredentialRequest.CredentialDefinitionId].SchemaId;
        SchemaRecord schemaRecord = SchemaState.Schemas[schemaId];
        foreach (string name in schemaRecord.AttributeNames)
        {
          OfferCredentialRequest
            .CredentialPreviewAttributes
            .Add
            (
              new CredentialPreviewAttribute(name, string.Empty)
            );
        }
      }
    }


    protected async Task CancelClick() =>
      _ = await Mediator.Send(new ChangeRouteAction { NewRoute = Pages.Index.RouteTemplate });

    protected async Task HandleValidSubmit()
    {
      _ = await Mediator.Send(new OfferCredentialAction { OfferCredentialRequest = OfferCredentialRequest });
      _ = await Mediator.Send(new ChangeRouteAction { NewRoute = Pages.Index.RouteTemplate });
    }

    protected override async Task OnInitializedAsync()
    {
      _ = await Mediator.Send(new FetchConnectionsAction());
      _ = await Mediator.Send(new FetchCredentialDefinitionsAction());
      _ = await Mediator.Send(new FetchSchemasAction());

      OfferCredentialRequest = new OfferCredentialRequest();
      OfferCredentialRequest.CredentialDefinitionId = 
        CredentialDefinitionState.CredentialDefinitions.Values.FirstOrDefault()?.Id;
      if(!string.IsNullOrEmpty(OfferCredentialRequest.CredentialDefinitionId))
      {
        OnCredentialDefintionSelect();
      }

      await base.OnInitializedAsync();
    }


  }
}
