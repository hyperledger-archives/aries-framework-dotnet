namespace BlazorHosted.Features.PresentProofs.Pages
{
  using Hyperledger.Aries.Features.DidExchange;
  using Hyperledger.Aries.Features.PresentProof;
  using Hyperledger.Aries.Models.Records;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using static BlazorHosted.Features.Connections.ConnectionState;
  using static BlazorHosted.Features.CredentialDefinitions.CredentialDefinitionState;
  using static BlazorHosted.Features.PresentProofs.PresentProofState;
  using static BlazorHosted.Features.Schemas.SchemaState;
  using static BlazorState.Features.Routing.RouteState;

  public partial class Create
  {
    public const string RouteTemplate = "/proofs/create";

    private IReadOnlyList<ConnectionRecord> Connections => ConnectionState.ConnectionsAsList;
    public string CredentialDefinitionId { get; set; }
    public SendRequestForProofRequest SendRequestForProofRequest { get; set; }
    private IReadOnlyList<DefinitionRecord> CredentialDefintions => CredentialDefinitionState.CredentialDefinitionsAsList;

    public static string GetRoute() => RouteTemplate;

    protected async Task CancelClick() =>
      _ = await Mediator.Send(new ChangeRouteAction { NewRoute = Pages.Index.RouteTemplate });

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

    protected void OnCredentialDefintionSelect()
    {
      Console.WriteLine("===OnCredentialDefintionSelect===");
      SendRequestForProofRequest.ProofRequest.RequestedAttributes.Clear();
      Console.WriteLine("===OnCredentialDefintionSelect===.2");
      if (!string.IsNullOrEmpty(CredentialDefinitionId))
      {
        DefinitionRecord definitionRecord = CredentialDefinitionState.CredentialDefinitions[CredentialDefinitionId];
        string schemaId = definitionRecord.SchemaId;
        SchemaRecord schemaRecord = SchemaState.Schemas[schemaId];
        SendRequestForProofRequest.ProofRequest.Name = schemaRecord.Name;
        SendRequestForProofRequest.ProofRequest.Version = schemaRecord.Version;
        foreach (string name in schemaRecord.AttributeNames)
        {
          var restrictions = new List<AttributeFilter>();

          var attributeFilter = new AttributeFilter
          {
            CredentialDefinitionId = definitionRecord.Id
          };

          restrictions.Add(attributeFilter);

          var proofAttributeInfo = new ProofAttributeInfo()
          {
            Name = name,
            Restrictions = restrictions,
          };

          SendRequestForProofRequest.ProofRequest
            .RequestedAttributes
            .Add($"0_{name}_uuid", proofAttributeInfo);
        }
      }
    }

    protected override async Task OnInitializedAsync()
    {
      Console.WriteLine("===OnInitializedAsync.start===");
      SendRequestForProofRequest = new SendRequestForProofRequest
      {
        ProofRequest = new ProofRequest { RequestedAttributes = new Dictionary<string, ProofAttributeInfo>() }
      };

      _ = await Mediator.Send(new FetchSchemasAction());
      _ = await Mediator.Send(new FetchConnectionsAction());
      _ = await Mediator.Send(new FetchCredentialDefinitionsAction());

      SendRequestForProofRequest.ConnectionId = Connections.FirstOrDefault()?.Id;
      CredentialDefinitionId = CredentialDefintions.FirstOrDefault()?.Id;
      if (!string.IsNullOrEmpty(CredentialDefinitionId))
      {
        OnCredentialDefintionSelect();
      }
      Console.WriteLine("===OnInitializedAsync.complete===");
      await base.OnInitializedAsync();
    }
  }
}
