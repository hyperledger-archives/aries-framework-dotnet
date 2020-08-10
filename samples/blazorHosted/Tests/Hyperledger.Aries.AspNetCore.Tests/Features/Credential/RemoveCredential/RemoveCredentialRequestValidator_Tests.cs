//namespace RemoveCredentialRequestValidator_
//{
//  using FluentAssertions;
//  using FluentValidation.Results;
//  using FluentValidation.TestHelper;
//  using Hyperledger.Aries.AspNetCore.Features.Credentials;

//  public class Validate_Should
//  {
//    private RemoveCredentialRequestValidator RemoveCredentialRequestValidator { get; set; }

//    public void Be_Valid()
//    {
//      var __requestName__Request = new RemoveCredentialRequest
//      {
//        // Set Valid values here
//        Days = 5
//      };

//      ValidationResult validationResult = RemoveCredentialRequestValidator.TestValidate(__requestName__Request);

//      validationResult.IsValid.Should().BeTrue();
//    }

//    public void Have_error_when_Days_are_negative() => RemoveCredentialRequestValidator
//      .ShouldHaveValidationErrorFor(aRemoveCredentialRequest => aRemoveCredentialRequest.Days, -1);

//    public void Setup() => RemoveCredentialRequestValidator = new RemoveCredentialRequestValidator();
//  }
//}
