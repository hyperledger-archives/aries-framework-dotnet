namespace SendMessageHandler
{
  using System.Threading.Tasks;
  using System.Text.Json;
  using Microsoft.AspNetCore.Mvc.Testing;
  using BlazorHosted.Server.Integration.Tests.Infrastructure;
  using BlazorHosted.Features.Connections;
  using BlazorHosted.Server;
  using FluentAssertions;

  public class Handle_Returns : BaseTest
  {
    private readonly SendMessageRequest SendMessageRequest;

    public Handle_Returns
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerOptions aJsonSerializerOptions
    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
    {
      SendMessageRequest = new SendMessageRequest { Days = 10 };
    }

    public async Task SendMessageResponse()
    {
      SendMessageResponse SendMessageResponse = await Send(SendMessageRequest);

      ValidateSendMessageResponse(SendMessageResponse);
    }

    private void ValidateSendMessageResponse(SendMessageResponse aSendMessageResponse)
    {
      aSendMessageResponse.CorrelationId.Should().Be(SendMessageRequest.CorrelationId);
      // check Other properties here
    }

  }
}