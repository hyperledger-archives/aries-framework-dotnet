namespace GetWalletEndpoint
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
    private readonly GetWalletRequest GetWalletRequest;

    public Returns
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerSettings aJsonSerializerSettings
    ) : base(aWebApplicationFactory, aJsonSerializerSettings)
    {
      GetWalletRequest = new GetWalletRequest();
    }

    public async Task GetWalletResponse_using_Json_Net()
    {
      GetWalletResponse getWalletResponse =
        await GetJsonAsync<GetWalletResponse>(GetWalletRequest.GetRoute());

      ValidateGetWalletResponse(GetWalletRequest, getWalletResponse);
    }

    public async Task GetWalletResponse_using_System_Text_Json()
    {
      GetWalletResponse getWalletResponse =
        await HttpClient.GetFromJsonAsync<GetWalletResponse>(GetWalletRequest.GetRoute());

      ValidateGetWalletResponse(GetWalletRequest, getWalletResponse);
    }

    // There are no validation requirements for this request
    public void ValidationError() { }
  }
}
