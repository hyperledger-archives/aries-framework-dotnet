namespace SendMessageRequestValidator_
{
  using FluentAssertions;
  using FluentValidation.Results;
  using FluentValidation.TestHelper;
  using BlazorHosted.Features.Connections;

  public class Validate_Should
  {
    private SendMessageRequestValidator SendMessageRequestValidator { get; set; }

    public void Be_Valid()
    {
      var __requestName__Request = new SendMessageRequest
      {
        // Set Valid values here
        Days = 5
      };

      ValidationResult validationResult = SendMessageRequestValidator.TestValidate(__requestName__Request);

      validationResult.IsValid.Should().BeTrue();
    }

    public void Have_error_when_Days_are_negative() => SendMessageRequestValidator
      .ShouldHaveValidationErrorFor(aSendMessageRequest => aSendMessageRequest.Days, -1);

    public void Setup() => SendMessageRequestValidator = new SendMessageRequestValidator();
  }
}
