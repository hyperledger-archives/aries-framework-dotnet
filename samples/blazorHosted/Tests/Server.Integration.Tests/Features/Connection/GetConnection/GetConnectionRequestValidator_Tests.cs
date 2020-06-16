namespace GetConnectionRequestValidator_
{
  using FluentAssertions;
  using FluentValidation.Results;
  using FluentValidation.TestHelper;
  using BlazorHosted.Features.Connections;

  public class Validate_Should
  {
    private GetConnectionRequestValidator GetConnectionRequestValidator { get; set; }

    public void Be_Valid()
    {
      var __requestName__Request = new GetConnectionRequest
      {
        // Set Valid values here
        Days = 5
      };

      ValidationResult validationResult = GetConnectionRequestValidator.TestValidate(__requestName__Request);

      validationResult.IsValid.Should().BeTrue();
    }

    public void Have_error_when_Days_are_negative() => GetConnectionRequestValidator
      .ShouldHaveValidationErrorFor(aGetConnectionRequest => aGetConnectionRequest.Days, -1);

    public void Setup() => GetConnectionRequestValidator = new GetConnectionRequestValidator();
  }
}
