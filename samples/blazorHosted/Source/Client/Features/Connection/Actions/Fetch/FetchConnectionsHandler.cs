namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using BlazorState;
  using MediatR;
  using System.Net.Http;
  using System.Threading;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using Newtonsoft.Json;
  using System.Linq;

  internal partial class ConnectionState
  {
    public class FetchConnectionsHandler : BaseHandler<FetchConnectionsAction>
    {
      private readonly HttpClient HttpClient;

      public FetchConnectionsHandler(IStore aStore, HttpClient aHttpClient) : base(aStore)
      {
        HttpClient = aHttpClient;
      }

      public override async Task<Unit> Handle
      (
        FetchConnectionsAction aFetchConnectionsAction,
        CancellationToken aCancellationToken
      )
      {
        var getConnectionsRequest = new GetConnectionsRequest();
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(getConnectionsRequest.GetRoute());
        string json = await httpResponseMessage.Content.ReadAsStringAsync();
        GetConnectionsResponse getConnectionsResponse = JsonConvert.DeserializeObject<GetConnectionsResponse>(json);

        ConnectionState._ConnectionRecords = getConnectionsResponse.ConnectionRecords
          .ToDictionary(aConnectionRecord => aConnectionRecord.Id, aConnectionRecord => aConnectionRecord);

        return Unit.Value;
      }
    }
  }
}
