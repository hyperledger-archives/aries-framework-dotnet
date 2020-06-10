namespace CreateInvitationRequestValidator_
{
  using FluentAssertions;
  using FluentValidation.Results;
  using FluentValidation.TestHelper;
  using BlazorHosted.Features.Connections;

  public class Validate_Should
  {
    private CreateInvitationRequestValidator CreateInvitationRequestValidator { get; set; }

    public void Be_Valid()
    {
      var __requestName__Request = new CreateInvitationRequest
      {
        // Set Valid values here
        Days = 5
      };

      ValidationResult validationResult = CreateInvitationRequestValidator.TestValidate(__requestName__Request);

      validationResult.IsValid.Should().BeTrue();
    }

    public void Have_error_when_Days_are_negative() => CreateInvitationRequestValidator
      .ShouldHaveValidationErrorFor(aCreateInvitationRequest => aCreateInvitationRequest.Days, -1);

    public void Setup() => CreateInvitationRequestValidator = new CreateInvitationRequestValidator();
  }
}
