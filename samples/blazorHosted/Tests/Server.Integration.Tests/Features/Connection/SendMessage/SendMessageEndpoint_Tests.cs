namespace SendMessageEndpoint
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
    private readonly SendMessageRequest SendMessageRequest;

    public Returns
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerOptions aJsonSerializerOptions
    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
    {
      SendMessageRequest = new SendMessageRequest { Days = 10 };
    }

    public async Task SendMessageResponse()
    {
      SendMessageResponse SendMessageResponse =
        await GetJsonAsync<SendMessageResponse>(SendMessageRequest.RouteFactory);

      ValidateSendMessageResponse(SendMessageResponse);
    }

    public async Task ValidationError()
    {
      // Set invalid value
      SendMessageRequest.Days = -1;

      HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(SendMessageRequest.RouteFactory);

      string json = await httpResponseMessage.Content.ReadAsStringAsync();

      httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
      json.Should().Contain("errors");
      json.Should().Contain(nameof(SendMessageRequest.Days));
    }

    private void ValidateSendMessageResponse(SendMessageResponse aSendMessageResponse)
    {
      aSendMessageResponse.CorrelationId.Should().Be(SendMessageRequest.CorrelationId);
      // check Other properties here
    }
  }
}