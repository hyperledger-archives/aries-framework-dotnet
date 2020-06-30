namespace GetWalletHandler
{
  using BlazorHosted.Features.Wallets;
  using BlazorHosted.Server;
  using BlazorHosted.Server.Integration.Tests.Infrastructure;
  using Microsoft.AspNetCore.Mvc.Testing;
  using Newtonsoft.Json;
  using System.Threading.Tasks;

  public class Handle_Returns : BaseTest
  {
    private readonly GetWalletRequest GetWalletRequest;

    public Handle_Returns
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerSettings aJsonSerializerSettings
    ) : base(aWebApplicationFactory, aJsonSerializerSettings)
    {
      GetWalletRequest = new GetWalletRequest();
    }

    public async Task GetWalletResponse()
    {
      GetWalletResponse getWalletResponse = await Send(GetWalletRequest);

      TestHelpers.GetWalletTestHelper.ValidateGetWalletResponse(GetWalletRequest, getWalletResponse);
    }
  }
}
