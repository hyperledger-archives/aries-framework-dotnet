//namespace OfferCredentialRequestValidator_
//{
//  using FluentAssertions;
//  using FluentValidation.Results;
//  using FluentValidation.TestHelper;
//  using Hyperledger.Aries.AspNetCore.Features.IssueCredentials;

//  public class Validate_Should
//  {
//    private OfferCredentialRequestValidator OfferCredentialRequestValidator { get; set; }

//    public void Be_Valid()
//    {
//      var __requestName__Request = new OfferCredentialRequest
//      {
//        // Set Valid values here
//        Days = 5
//      };

//      ValidationResult validationResult = OfferCredentialRequestValidator.TestValidate(__requestName__Request);

//      validationResult.IsValid.Should().BeTrue();
//    }

//    public void Have_error_when_Days_are_negative() => OfferCredentialRequestValidator
//      .ShouldHaveValidationErrorFor(aOfferCredentialRequest => aOfferCredentialRequest.Days, -1);

//    public void Setup() => OfferCredentialRequestValidator = new OfferCredentialRequestValidator();
//  }
//}
