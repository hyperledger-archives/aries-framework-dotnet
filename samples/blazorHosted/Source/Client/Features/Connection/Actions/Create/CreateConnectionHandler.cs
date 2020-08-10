namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using BlazorState;
  using Hyperledger.Aries.Features.DidExchange;
  using MediatR;
  using Newtonsoft.Json;
  using System;
  using System.Net.Http;
  using System.Net.Http.Json;
  using System.Threading;
  using System.Threading.Tasks;

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
        var inviteConfiguration = new InviteConfiguration();
        inviteConfiguration.MyAlias.Name = WalletState.ProvisioningRecord.Owner.Name;
        inviteConfiguration.AutoAcceptConnection = true;

        var createInvitationRequest = new CreateInvitationRequest(inviteConfiguration);
        
        HttpResponseMessage httpResponseMessage =
          await HttpClient.PostAsJsonAsync<CreateInvitationRequest>
          (
            createInvitationRequest.GetRoute(),
            createInvitationRequest
          );

        httpResponseMessage.EnsureSuccessStatusCode();
        string json = await httpResponseMessage.Content.ReadAsStringAsync();

        CreateInvitationResponse createConnectionResponse =
          JsonConvert.DeserializeObject<CreateInvitationResponse>(json);

        ConnectionState.InvitationUrl = createConnectionResponse.InvitationUrl;

        ConnectionState.InvitationQrUri =
          $"https://chart.googleapis.com/chart?cht=qr&chs=300x300&chld=L|0&chl=" +
          $"{Uri.EscapeDataString(createConnectionResponse.InvitationUrl)}";

        return Unit.Value;
      }
    }
  }
}
