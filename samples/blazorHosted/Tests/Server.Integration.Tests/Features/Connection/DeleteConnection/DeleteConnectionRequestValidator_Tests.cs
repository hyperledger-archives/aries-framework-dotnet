namespace DeleteConnectionRequestValidator_
{
  using FluentAssertions;
  using FluentValidation.Results;
  using FluentValidation.TestHelper;
  using BlazorHosted.Features.Connections;

  public class Validate_Should
  {
    private DeleteConnectionRequestValidator DeleteConnectionRequestValidator { get; set; }

    public void Be_Valid()
    {
      var __requestName__Request = new DeleteConnectionRequest
      {
        // Set Valid values here
        Days = 5
      };

      ValidationResult validationResult = DeleteConnectionRequestValidator.TestValidate(__requestName__Request);

      validationResult.IsValid.Should().BeTrue();
    }

    public void Have_error_when_Days_are_negative() => DeleteConnectionRequestValidator
      .ShouldHaveValidationErrorFor(aDeleteConnectionRequest => aDeleteConnectionRequest.Days, -1);

    public void Setup() => DeleteConnectionRequestValidator = new DeleteConnectionRequestValidator();
  }
}
