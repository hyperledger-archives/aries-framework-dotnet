namespace GetCredentialEndpoint
{
  using FluentAssertions;
  using Microsoft.AspNetCore.Mvc.Testing;
  using System.Net;
  using System.Net.Http;
  using System.Text.Json;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Credentials;
  using BlazorHosted.Server.Integration.Tests.Infrastructure;
  using BlazorHosted.Server;

  public class Returns : BaseTest
  {
    private readonly GetCredentialRequest GetCredentialRequest;

    public Returns
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerOptions aJsonSerializerOptions
    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
    {
      GetCredentialRequest = new GetCredentialRequest { Days = 10 };
    }

    public async Task GetCredentialResponse()
    {
      GetCredentialResponse GetCredentialResponse =
        await GetJsonAsync<GetCredentialResponse>(GetCredentialRequest.RouteFactory);

      ValidateGetCredentialResponse(GetCredentialResponse);
    }

    public async Task ValidationError()
    {
      // Set invalid value
      GetCredentialRequest.Days = -1;

      HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(GetCredentialRequest.RouteFactory);

      string json = await httpResponseMessage.Content.ReadAsStringAsync();

      httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
      json.Should().Contain("errors");
      json.Should().Contain(nameof(GetCredentialRequest.Days));
    }

    private void ValidateGetCredentialResponse(GetCredentialResponse aGetCredentialResponse)
    {
      aGetCredentialResponse.CorrelationId.Should().Be(GetCredentialRequest.CorrelationId);
      // check Other properties here
    }
  }
}