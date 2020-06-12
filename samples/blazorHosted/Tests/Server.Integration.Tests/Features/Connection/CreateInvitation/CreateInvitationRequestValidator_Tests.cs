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
        Alias = "Alice"
      };

      ValidationResult validationResult = CreateInvitationRequestValidator.TestValidate(__requestName__Request);

      validationResult.IsValid.Should().BeTrue();
    }

    public void Have_error_when_Days_are_negative() => CreateInvitationRequestValidator
      .ShouldHaveValidationErrorFor(aCreateInvitationRequest => aCreateInvitationRequest.Alias, "" );

    public void Setup() => CreateInvitationRequestValidator = new CreateInvitationRequestValidator();
  }
}
