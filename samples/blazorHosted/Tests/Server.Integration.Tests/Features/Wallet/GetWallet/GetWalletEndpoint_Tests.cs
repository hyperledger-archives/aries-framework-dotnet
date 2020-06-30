namespace GetWalletEndpoint
{
  using BlazorHosted.Features.Wallets;
  using BlazorHosted.Server;
  using BlazorHosted.Server.Integration.Tests.Infrastructure;
  using FluentAssertions;
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

      TestHelpers.GetWalletTestHelper.ValidateGetWalletResponse(GetWalletRequest, getWalletResponse);
    }

    public async Task GetWalletResponse_using_System_Text_Json()
    {
      GetWalletResponse getWalletResponse =
        await HttpClient.GetFromJsonAsync<GetWalletResponse>(GetWalletRequest.GetRoute());

      TestHelpers.GetWalletTestHelper.ValidateGetWalletResponse(GetWalletRequest, getWalletResponse);
    }

    // There are no validation requirements for this request
    public void ValidationError() { }

  }
}
