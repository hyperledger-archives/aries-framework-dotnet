namespace GetWalletHandler
{
  using Hyperledger.Aries.AspNetCore.Features.Wallets;
  using Hyperledger.Aries.AspNetCore.Server;
  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
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

      ValidateGetWalletResponse(GetWalletRequest, getWalletResponse);
    }

    public async Task Setup() => await ResetAgent();
  }
}
