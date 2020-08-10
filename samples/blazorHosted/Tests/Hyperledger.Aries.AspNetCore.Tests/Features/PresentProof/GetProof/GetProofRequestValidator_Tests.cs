//namespace GetProofRequestValidator_
//{
//  using FluentAssertions;
//  using FluentValidation.Results;
//  using FluentValidation.TestHelper;
//  using Hyperledger.Aries.AspNetCore.Features.PresentProofs;

//  public class Validate_Should
//  {
//    private GetProofRequestValidator GetProofRequestValidator { get; set; }

//    public void Be_Valid()
//    {
//      var __requestName__Request = new GetProofRequest
//      {
//        // Set Valid values here
//        Days = 5
//      };

//      ValidationResult validationResult = GetProofRequestValidator.TestValidate(__requestName__Request);

//      validationResult.IsValid.Should().BeTrue();
//    }

//    public void Have_error_when_Days_are_negative() => GetProofRequestValidator
//      .ShouldHaveValidationErrorFor(aGetProofRequest => aGetProofRequest.Days, -1);

//    public void Setup() => GetProofRequestValidator = new GetProofRequestValidator();
//  }
//}
