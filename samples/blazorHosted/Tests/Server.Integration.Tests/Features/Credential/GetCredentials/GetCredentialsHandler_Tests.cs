namespace GetCredentialsHandler
{
  using System.Threading.Tasks;
  using System.Text.Json;
  using Microsoft.AspNetCore.Mvc.Testing;
  using BlazorHosted.Server.Integration.Tests.Infrastructure;
  using BlazorHosted.Features.Credentials;
  using BlazorHosted.Server;
  using FluentAssertions;

  public class Handle_Returns : BaseTest
  {
    private readonly GetCredentialsRequest GetCredentialsRequest;

    public Handle_Returns
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerOptions aJsonSerializerOptions
    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
    {
      GetCredentialsRequest = new GetCredentialsRequest { Days = 10 };
    }

    public async Task GetCredentialsResponse()
    {
      GetCredentialsResponse GetCredentialsResponse = await Send(GetCredentialsRequest);

      ValidateGetCredentialsResponse(GetCredentialsResponse);
    }

    private void ValidateGetCredentialsResponse(GetCredentialsResponse aGetCredentialsResponse)
    {
      aGetCredentialsResponse.CorrelationId.Should().Be(GetCredentialsRequest.CorrelationId);
      // check Other properties here
    }

  }
}