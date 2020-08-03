//namespace AcceptProofRequestRequestValidator_
//{
//  using FluentAssertions;
//  using FluentValidation.Results;
//  using FluentValidation.TestHelper;
//  using Hyperledger.Aries.AspNetCore.Features.PresentProofs;

//  public class Validate_Should
//  {
//    private AcceptProofRequestRequestValidator AcceptProofRequestRequestValidator { get; set; }

//    public void Be_Valid()
//    {
//      var __requestName__Request = new AcceptProofRequestRequest
//      {
//        // Set Valid values here
//        Days = 5
//      };

//      ValidationResult validationResult = AcceptProofRequestRequestValidator.TestValidate(__requestName__Request);

//      validationResult.IsValid.Should().BeTrue();
//    }

//    public void Have_error_when_Days_are_negative() => AcceptProofRequestRequestValidator
//      .ShouldHaveValidationErrorFor(aAcceptProofRequestRequest => aAcceptProofRequestRequest.Days, -1);

//    public void Setup() => AcceptProofRequestRequestValidator = new AcceptProofRequestRequestValidator();
//  }
//}
