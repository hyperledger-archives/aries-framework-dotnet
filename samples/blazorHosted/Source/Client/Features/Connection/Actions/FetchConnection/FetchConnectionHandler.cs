namespace BlazorHosted.Features.Connections
{
  using BlazorHosted.Features.Bases;
  using BlazorState;
  using MediatR;
  using Newtonsoft.Json;
  using System.Net.Http;
  using System.Threading;
  using System.Threading.Tasks;

  internal partial class ConnectionState
  {
    public class FetchConnectionHandler : BaseHandler<FetchConnectionAction>
    {
      private readonly HttpClient HttpClient;

      public FetchConnectionHandler(IStore aStore, HttpClient aHttpClient) : base(aStore)
      {
        HttpClient = aHttpClient;
      }

      public override async Task<Unit> Handle
      (
        FetchConnectionAction aFetchConnectionAction,
        CancellationToken aCancellationToken
      )
      {
        var getConnectionRequest = new GetConnectionRequest(aFetchConnectionAction.ConnectionId);
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(getConnectionRequest.GetRoute());
        string json = await httpResponseMessage.Content.ReadAsStringAsync();
        GetConnectionResponse getConnectionResponse = JsonConvert.DeserializeObject<GetConnectionResponse>(json);
        if(getConnectionResponse.ConnectionRecord != null)
        {
          ConnectionState._ConnectionRecords[getConnectionResponse.ConnectionRecord.Id] = getConnectionResponse.ConnectionRecord;
        }
        return Unit.Value;
      }
    }
  }
}
