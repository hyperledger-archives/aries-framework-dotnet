namespace DeleteConnectionRequestValidator_
{
  using FluentAssertions;
  using FluentValidation.Results;
  using FluentValidation.TestHelper;
  using Hyperledger.Aries.AspNetCore.Features.Connections;
  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
  using Microsoft.AspNetCore.Mvc.Testing;
  using Newtonsoft.Json;
  using Hyperledger.Aries.AspNetCore.Server;

  public class Validate_Should: BaseTest
  {
    private DeleteConnectionRequestValidator DeleteConnectionRequestValidator { get; set; }
    private DeleteConnectionRequest DeleteConnectionRequest { get; set; }

    public Validate_Should
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerSettings aJsonSerializerSettings
    ) : base(aWebApplicationFactory, aJsonSerializerSettings)
    {
      DeleteConnectionRequestValidator = new DeleteConnectionRequestValidator();
      DeleteConnectionRequest = new DeleteConnectionRequest("ConnectionId");
    }

    public void Be_Valid()
    {
      ValidationResult validationResult = DeleteConnectionRequestValidator.TestValidate(DeleteConnectionRequest);

      validationResult.IsValid.Should().BeTrue();
    }

    public void Have_error_when_ConnectionId_is_empty() => DeleteConnectionRequestValidator
      .ShouldHaveValidationErrorFor(aDeleteConnectionRequest => aDeleteConnectionRequest.ConnectionId, string.Empty);

    public void Have_error_when_ConnectionId_is_null()
    {
      DeleteConnectionRequest.ConnectionId = null;
      DeleteConnectionRequestValidator
        .ShouldHaveValidationErrorFor
        (
          aDeleteConnectionRequest => aDeleteConnectionRequest.ConnectionId, 
          DeleteConnectionRequest
        );
    }
  }
}
