//namespace GetCredentialRequestValidator_
//{
//  using FluentAssertions;
//  using FluentValidation.Results;
//  using FluentValidation.TestHelper;
//  using Hyperledger.Aries.AspNetCore.Features.Credentials;

//  public class Validate_Should
//  {
//    private GetCredentialRequestValidator GetCredentialRequestValidator { get; set; }

//    public void Be_Valid()
//    {
//      var __requestName__Request = new GetCredentialRequest
//      {
//        // Set Valid values here
//        Days = 5
//      };

//      ValidationResult validationResult = GetCredentialRequestValidator.TestValidate(__requestName__Request);

//      validationResult.IsValid.Should().BeTrue();
//    }

//    public void Have_error_when_Days_are_negative() => GetCredentialRequestValidator
//      .ShouldHaveValidationErrorFor(aGetCredentialRequest => aGetCredentialRequest.Days, -1);

//    public void Setup() => GetCredentialRequestValidator = new GetCredentialRequestValidator();
//  }
//}
