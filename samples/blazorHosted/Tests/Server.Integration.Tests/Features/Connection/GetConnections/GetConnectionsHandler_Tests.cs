namespace GetConnectionsHandler
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
    private readonly GetConnectionsRequest GetConnectionsRequest;

    public Handle_Returns
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerOptions aJsonSerializerOptions
    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
    {
      GetConnectionsRequest = new GetConnectionsRequest { Days = 10 };
    }

    public async Task GetConnectionsResponse()
    {
      GetConnectionsResponse GetConnectionsResponse = await Send(GetConnectionsRequest);

      ValidateGetConnectionsResponse(GetConnectionsResponse);
    }

    private void ValidateGetConnectionsResponse(GetConnectionsResponse aGetConnectionsResponse)
    {
      aGetConnectionsResponse.CorrelationId.Should().Be(GetConnectionsRequest.CorrelationId);
      // check Other properties here
    }

  }
}