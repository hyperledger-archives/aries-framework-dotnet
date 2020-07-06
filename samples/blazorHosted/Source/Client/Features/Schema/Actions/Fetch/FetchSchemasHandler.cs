namespace Hyperledger.Aries.AspNetCore.Features.Schemas
{
  using BlazorState;
  using MediatR;
  using System.Net.Http;
  using System.Threading;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using Newtonsoft.Json;
  using System.Linq;

  internal partial class SchemaState
  {
    public class FetchSchemasHandler : BaseHandler<FetchSchemasAction>
    {
      private readonly HttpClient HttpClient;

      public FetchSchemasHandler(IStore aStore, HttpClient aHttpClient) : base(aStore)
      {
        HttpClient = aHttpClient;
      }

      public override async Task<Unit> Handle
      (
        FetchSchemasAction aFetchConnectionsAction,
        CancellationToken aCancellationToken
      )
      {
        var getSchemasRequest = new GetSchemasRequest();
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(getSchemasRequest.GetRoute());
        string json = await httpResponseMessage.Content.ReadAsStringAsync();
        GetSchemasResponse getSchemasResponse = JsonConvert.DeserializeObject<GetSchemasResponse>(json);

        SchemaState._SchemaRecords = getSchemasResponse.SchemaRecords
          .ToDictionary(aSchemaRecord => aSchemaRecord.Id, aSchemaRecord => aSchemaRecord);

        return Unit.Value;
      }
    }
  }
}
