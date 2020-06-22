namespace BlazorHosted.Features.Connections
{
  using BlazorState;
  using MediatR;
  using System.Net.Http;
  using System.Net.Http.Json;
  using System.Threading;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;
  using Newtonsoft.Json;

  internal partial class ConnectionState
  {
    public class CreateConnectionHandler : BaseHandler<CreateConnectionAction>
    {
      private readonly HttpClient HttpClient;

      public CreateConnectionHandler
      (
        IStore aStore, 
        HttpClient aHttpClient
      ) : base(aStore)
      {
        HttpClient = aHttpClient;
      }


      public override async Task<Unit> Handle
      (
        CreateConnectionAction aCreateConnectionAction,
        CancellationToken aCancellationToken
      )
      {
        HttpResponseMessage httpResponseMessage =
          await HttpClient.PostAsJsonAsync<CreateInvitationRequest>
          (
            aCreateConnectionAction.CreateInvitationRequest.GetRoute(),
            aCreateConnectionAction.CreateInvitationRequest
          );

        httpResponseMessage.EnsureSuccessStatusCode();

        string json = await httpResponseMessage.Content.ReadAsStringAsync();

        CreateInvitationResponse createConnectionResponse =
          JsonConvert.DeserializeObject<CreateInvitationResponse>(json);

        ConnectionState.InvitationUrl = createConnectionResponse.InvitationUrl;
        
        return Unit.Value;
      }
    }
  }
}
