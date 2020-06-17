namespace CreateSchemaHandler
{
  using System.Threading.Tasks;
  using System.Text.Json;
  using Microsoft.AspNetCore.Mvc.Testing;
  using BlazorHosted.Server.Integration.Tests.Infrastructure;
  using BlazorHosted.Features.Schemas;
  using BlazorHosted.Server;
  using FluentAssertions;

  public class Handle_Returns : BaseTest
  {
    private readonly CreateSchemaRequest CreateSchemaRequest;

    public Handle_Returns
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerOptions aJsonSerializerOptions
    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
    {
      CreateSchemaRequest = new CreateSchemaRequest { Days = 10 };
    }

    public async Task CreateSchemaResponse()
    {
      CreateSchemaResponse CreateSchemaResponse = await Send(CreateSchemaRequest);

      ValidateCreateSchemaResponse(CreateSchemaResponse);
    }

    private void ValidateCreateSchemaResponse(CreateSchemaResponse aCreateSchemaResponse)
    {
      aCreateSchemaResponse.CorrelationId.Should().Be(CreateSchemaRequest.CorrelationId);
      // check Other properties here
    }

  }
}