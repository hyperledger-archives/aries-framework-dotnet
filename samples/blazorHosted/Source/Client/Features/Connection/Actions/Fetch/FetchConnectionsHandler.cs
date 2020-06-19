namespace BlazorHosted.Features.Connections
{
  using BlazorState;
  using MediatR;
  using System.Net.Http;
  using System.Net.Http.Json;
  using System.Threading;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;
  using BlazorHosted.Features.WeatherForecasts;
  using System.Linq;
  using System;
  using Newtonsoft.Json;
  using static BlazorHosted.Features.Connections.ConnectionState;

  internal partial class ConnectionsState
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
        HttpResponseMessage x = await HttpClient.GetAsync(getConnectionsRequest.RouteFactory);
        string json = await x.Content.ReadAsStringAsync();
        GetConnectionsResponse getConnectionsResponse = JsonConvert.DeserializeObject<GetConnectionsResponse>(json);

        ConnectionState._ConnectionRecords = getConnectionsResponse.ConnectionRecords;

        return Unit.Value;
      }
    }
  }
}
