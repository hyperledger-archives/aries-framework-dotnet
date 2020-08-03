namespace ResetWalletEndpoint
{
  using Hyperledger.Aries.AspNetCore.Features.Wallets;
  using Hyperledger.Aries.AspNetCore.Server;
  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
  using Microsoft.AspNetCore.Mvc.Testing;
  using Newtonsoft.Json;
  using System.Net.Http;
  using System.Net.Http.Json;
  using System.Threading.Tasks;

  public class Returns : BaseTest
  {
    private readonly ResetWalletRequest ResetWalletRequest;

    public Returns
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerSettings aJsonSerializerSettings
    ) : base(aWebApplicationFactory, aJsonSerializerSettings)
    {
      ResetWalletRequest = new ResetWalletRequest();
    }

    public async Task ResetWalletResponse_using_Json_Net()
    {
      ResetWalletResponse resetWalletResponse =
        await Post<ResetWalletResponse>(ResetWalletRequest.GetRoute(), ResetWalletRequest);

      ValidateResetWalletResponse(ResetWalletRequest, resetWalletResponse);
    }

    public async Task ResetWalletResponse_using_System_Text_Json()
    {
      HttpResponseMessage httpResponseMessage =
        await HttpClient.PostAsJsonAsync<ResetWalletRequest>(ResetWalletRequest.GetRoute(), ResetWalletRequest);

      ResetWalletResponse resetWalletResponse =
        await httpResponseMessage.Content.ReadFromJsonAsync<ResetWalletResponse>();

      ValidateResetWalletResponse(ResetWalletRequest, resetWalletResponse);
    }

    // There are no validation requirements for this request
    public void ValidationError() { }
  }
}
