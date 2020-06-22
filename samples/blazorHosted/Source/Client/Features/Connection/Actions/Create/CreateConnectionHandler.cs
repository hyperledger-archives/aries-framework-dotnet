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
  using System;

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
        Console.WriteLine("CreateConnectionHandler.0");
        var createInvitationRequest = new CreateInvitationRequest();

        HttpResponseMessage httpResponseMessage =
          await HttpClient.PostAsJsonAsync<CreateInvitationRequest>
          (
            createInvitationRequest.GetRoute(),
            createInvitationRequest
          );

        Console.WriteLine("CreateConnectionHandler.1");
        httpResponseMessage.EnsureSuccessStatusCode();
        Console.WriteLine("CreateConnectionHandler.2");

        string json = await httpResponseMessage.Content.ReadAsStringAsync();
        Console.WriteLine("CreateConnectionHandler.3");

        CreateInvitationResponse createConnectionResponse =
          JsonConvert.DeserializeObject<CreateInvitationResponse>(json);

        Console.WriteLine("CreateConnectionHandler.4");
        ConnectionState.InvitationUrl = createConnectionResponse.InvitationUrl;
        Console.WriteLine("CreateConnectionHandler.5");
        ConnectionState.InvitationQrUri =
          $"https://chart.googleapis.com/chart?cht=qr&chs=300x300&chld=L|0&chl=" +
          $"{Uri.EscapeDataString(createConnectionResponse.InvitationUrl)}";
        Console.WriteLine("CreateConnectionHandler.6");
        return Unit.Value;
      }
    }
  }
}
