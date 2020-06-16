namespace GetConnectionsEndpoint
{
  using FluentAssertions;
  using Microsoft.AspNetCore.Mvc.Testing;
  using System.Net;
  using System.Net.Http;
  using System.Text.Json;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Connections;
  using BlazorHosted.Server.Integration.Tests.Infrastructure;
  using BlazorHosted.Server;

  public class Returns : BaseTest
  {
    private readonly GetConnectionsRequest GetConnectionsRequest;

    public Returns
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerOptions aJsonSerializerOptions
    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
    {
      GetConnectionsRequest = new GetConnectionsRequest { Days = 10 };
    }

    public async Task GetConnectionsResponse()
    {
      GetConnectionsResponse GetConnectionsResponse =
        await GetJsonAsync<GetConnectionsResponse>(GetConnectionsRequest.RouteFactory);

      ValidateGetConnectionsResponse(GetConnectionsResponse);
    }

    public async Task ValidationError()
    {
      // Set invalid value
      GetConnectionsRequest.Days = -1;

      HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(GetConnectionsRequest.RouteFactory);

      string json = await httpResponseMessage.Content.ReadAsStringAsync();

      httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
      json.Should().Contain("errors");
      json.Should().Contain(nameof(GetConnectionsRequest.Days));
    }

    private void ValidateGetConnectionsResponse(GetConnectionsResponse aGetConnectionsResponse)
    {
      aGetConnectionsResponse.CorrelationId.Should().Be(GetConnectionsRequest.CorrelationId);
      // check Other properties here
    }
  }
}