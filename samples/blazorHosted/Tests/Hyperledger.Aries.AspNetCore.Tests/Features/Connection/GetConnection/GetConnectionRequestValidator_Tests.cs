namespace GetConnectionRequestValidator_
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
    private GetConnectionRequest GetConnectionRequest { get; set; }
    private GetConnectionRequestValidator GetConnectionRequestValidator { get; set; }

    public Validate_Should
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerSettings aJsonSerializerSettings
    ) : base(aWebApplicationFactory, aJsonSerializerSettings)
    {
      GetConnectionRequestValidator = new GetConnectionRequestValidator();
      GetConnectionRequest = CreateValidGetConnectionRequest();
    }

    public void Have_error_when_ConnectionId_is_empty() => GetConnectionRequestValidator
      .ShouldHaveValidationErrorFor(aGetConnectionRequest => aGetConnectionRequest.ConnectionId, string.Empty);

    public void Have_error_when_ConnectionId_is_null()
    {
      GetConnectionRequest.ConnectionId = null;
      GetConnectionRequestValidator
        .ShouldHaveValidationErrorFor(aGetConnectionRequest => aGetConnectionRequest.ConnectionId, GetConnectionRequest);
    }

    public void Be_Valid()
    {
      ValidationResult validationResult = GetConnectionRequestValidator.TestValidate(GetConnectionRequest);

      validationResult.IsValid.Should().BeTrue();
    }
  }
}
