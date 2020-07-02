namespace GetConnectionsRequestValidator_
{
  using BlazorHosted.Features.Connections;
  using BlazorHosted.Server;
  using BlazorHosted.Server.Integration.Tests.Infrastructure;
  using FluentAssertions;
  using FluentValidation.Results;
  using FluentValidation.TestHelper;
  using Microsoft.AspNetCore.Mvc.Testing;
  using Newtonsoft.Json;

  public class Validate_Should : BaseTest
  {
    private GetConnectionsRequest GetConnectionsRequest { get; set; }
    private GetConnectionsRequestValidator GetConnectionsRequestValidator { get; set; }

    public Validate_Should
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerSettings aJsonSerializerSettings
    ) : base(aWebApplicationFactory, aJsonSerializerSettings)
    {
      GetConnectionsRequestValidator = new GetConnectionsRequestValidator();
      GetConnectionsRequest = CreateValidGetConnectionsRequest();
    }

    public void Be_Valid()
    {
      ValidationResult validationResult = GetConnectionsRequestValidator.TestValidate(GetConnectionsRequest);

      validationResult.IsValid.Should().BeTrue();
    }
  }
}
