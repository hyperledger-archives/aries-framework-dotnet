namespace GetConnectionsRequestValidator_
{
  using FluentAssertions;
  using FluentValidation.Results;
  using FluentValidation.TestHelper;
  using BlazorHosted.Features.Connections;

  public class Validate_Should
  {
    private GetConnectionsRequestValidator GetConnectionsRequestValidator { get; set; }

    public void Be_Valid()
    {
      var __requestName__Request = new GetConnectionsRequest
      {
        // Set Valid values here
        Days = 5
      };

      ValidationResult validationResult = GetConnectionsRequestValidator.TestValidate(__requestName__Request);

      validationResult.IsValid.Should().BeTrue();
    }

    public void Have_error_when_Days_are_negative() => GetConnectionsRequestValidator
      .ShouldHaveValidationErrorFor(aGetConnectionsRequest => aGetConnectionsRequest.Days, -1);

    public void Setup() => GetConnectionsRequestValidator = new GetConnectionsRequestValidator();
  }
}
