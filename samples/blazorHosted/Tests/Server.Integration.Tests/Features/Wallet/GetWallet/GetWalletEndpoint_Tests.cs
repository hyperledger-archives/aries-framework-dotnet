namespace GetWalletEndpoint
{
  using FluentAssertions;
  using Microsoft.AspNetCore.Mvc.Testing;
  using System.Net;
  using System.Net.Http;
  using System.Text.Json;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Wallets;
  using BlazorHosted.Server.Integration.Tests.Infrastructure;
  using BlazorHosted.Server;

  public class Returns : BaseTest
  {
    private readonly GetWalletRequest GetWalletRequest;

    public Returns
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerOptions aJsonSerializerOptions
    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
    {
      GetWalletRequest = new GetWalletRequest();
    }

    public async Task GetWalletResponse()
    {
      GetWalletResponse GetWalletResponse =
        await GetJsonAsync<GetWalletResponse>(GetWalletRequest.RouteFactory);

      ValidateGetWalletResponse(GetWalletResponse);
    }

    //public async Task ValidationError()
    //{
    //  // Set invalid value
    //  GetWalletRequest.Days = -1;

    //  HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(GetWalletRequest.RouteFactory);

    //  string json = await httpResponseMessage.Content.ReadAsStringAsync();

    //  httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    //  json.Should().Contain("errors");
    //  json.Should().Contain(nameof(GetWalletRequest.Days));
    //}

    private void ValidateGetWalletResponse(GetWalletResponse aGetWalletResponse)
    {
      aGetWalletResponse.CorrelationId.Should().Be(GetWalletRequest.CorrelationId);
      // check Other properties here
    }
  }
}