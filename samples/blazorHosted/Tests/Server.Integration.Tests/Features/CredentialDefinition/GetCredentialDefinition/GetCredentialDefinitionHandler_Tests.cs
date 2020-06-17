namespace GetCredentialDefinitionHandler
{
  using System.Threading.Tasks;
  using System.Text.Json;
  using Microsoft.AspNetCore.Mvc.Testing;
  using BlazorHosted.Server.Integration.Tests.Infrastructure;
  using BlazorHosted.Features.CredentialDefinitions;
  using BlazorHosted.Server;
  using FluentAssertions;

  public class Handle_Returns : BaseTest
  {
    private readonly GetCredentialDefinitionRequest GetCredentialDefinitionRequest;

    public Handle_Returns
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerOptions aJsonSerializerOptions
    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
    {
      GetCredentialDefinitionRequest = new GetCredentialDefinitionRequest { Days = 10 };
    }

    public async Task GetCredentialDefinitionResponse()
    {
      GetCredentialDefinitionResponse GetCredentialDefinitionResponse = await Send(GetCredentialDefinitionRequest);

      ValidateGetCredentialDefinitionResponse(GetCredentialDefinitionResponse);
    }

    private void ValidateGetCredentialDefinitionResponse(GetCredentialDefinitionResponse aGetCredentialDefinitionResponse)
    {
      aGetCredentialDefinitionResponse.CorrelationId.Should().Be(GetCredentialDefinitionRequest.CorrelationId);
      // check Other properties here
    }

  }
}