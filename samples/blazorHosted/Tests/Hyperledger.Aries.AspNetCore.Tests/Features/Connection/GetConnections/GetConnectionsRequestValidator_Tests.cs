namespace GetConnectionsRequestValidator_
{
  using Hyperledger.Aries.AspNetCore.Features.Connections;
  using Hyperledger.Aries.AspNetCore.Server;
  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
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
