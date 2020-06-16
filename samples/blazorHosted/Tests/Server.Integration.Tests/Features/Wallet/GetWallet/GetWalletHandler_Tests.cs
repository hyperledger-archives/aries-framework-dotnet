namespace GetWalletHandler
{
  using System.Threading.Tasks;
  using System.Text.Json;
  using Microsoft.AspNetCore.Mvc.Testing;
  using BlazorHosted.Server.Integration.Tests.Infrastructure;
  using BlazorHosted.Features.Wallets;
  using BlazorHosted.Server;
  using FluentAssertions;

  public class Handle_Returns : BaseTest
  {
    private readonly GetWalletRequest GetWalletRequest;

    public Handle_Returns
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerOptions aJsonSerializerOptions
    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
    {
      GetWalletRequest = new GetWalletRequest();
    }

    public async Task GetWalletResponse()
    {
      GetWalletResponse GetWalletResponse = await Send(GetWalletRequest);

      ValidateGetWalletResponse(GetWalletResponse);
    }

    private void ValidateGetWalletResponse(GetWalletResponse aGetWalletResponse)
    {
      aGetWalletResponse.CorrelationId.Should().Be(GetWalletRequest.CorrelationId);
      // check Other properties here
    }

  }
}