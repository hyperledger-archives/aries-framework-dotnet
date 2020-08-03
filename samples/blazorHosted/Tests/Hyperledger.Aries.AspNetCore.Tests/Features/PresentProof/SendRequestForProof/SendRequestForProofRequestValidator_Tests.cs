//namespace SendRequestForProofRequestValidator_
//{
//  using FluentAssertions;
//  using FluentValidation.Results;
//  using FluentValidation.TestHelper;
//  using Hyperledger.Aries.AspNetCore.Features.PresentProofs;

//  public class Validate_Should
//  {
//    private SendRequestForProofRequestValidator SendRequestForProofRequestValidator { get; set; }

//    public void Be_Valid()
//    {
//      var __requestName__Request = new SendRequestForProofRequest
//      {
//        // Set Valid values here
//        Days = 5
//      };

//      ValidationResult validationResult = SendRequestForProofRequestValidator.TestValidate(__requestName__Request);

//      validationResult.IsValid.Should().BeTrue();
//    }

//    public void Have_error_when_Days_are_negative() => SendRequestForProofRequestValidator
//      .ShouldHaveValidationErrorFor(aSendRequestForProofRequest => aSendRequestForProofRequest.Days, -1);

//    public void Setup() => SendRequestForProofRequestValidator = new SendRequestForProofRequestValidator();
//  }
//}
