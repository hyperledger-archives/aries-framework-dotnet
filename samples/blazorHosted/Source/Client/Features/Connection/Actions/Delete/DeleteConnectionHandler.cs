namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using BlazorState;
  using MediatR;
  using System.Net.Http;
  using System.Net.Http.Json;
  using System.Threading;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using Newtonsoft.Json;
  using System;

  internal partial class ConnectionState
  {
    public class DeleteConnectionHandler : BaseHandler<DeleteConnectionAction>
    {
      private readonly HttpClient HttpClient;

      public DeleteConnectionHandler
      (
        IStore aStore,
        HttpClient aHttpClient
      ) : base(aStore)
      {
        HttpClient = aHttpClient;
      }


      public override async Task<Unit> Handle
      (
        DeleteConnectionAction aDeleteConnectionAction,
        CancellationToken aCancellationToken
      )
      {
        var deleteConnectionRequest = 
          new DeleteConnectionRequest { ConnectionId = aDeleteConnectionAction.ConnectionId };

        HttpResponseMessage httpResponseMessage =
          await HttpClient.DeleteAsync(deleteConnectionRequest.GetRoute());

        httpResponseMessage.EnsureSuccessStatusCode();

        ConnectionState._ConnectionRecords.Remove(deleteConnectionRequest.ConnectionId);

        return Unit.Value;
      }
    }
  }
}
