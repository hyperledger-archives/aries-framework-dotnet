//namespace GetProofsRequestValidator_
//{
//  using FluentAssertions;
//  using FluentValidation.Results;
//  using FluentValidation.TestHelper;
//  using Hyperledger.Aries.AspNetCore.Features.PresentProofs;

//  public class Validate_Should
//  {
//    private GetProofsRequestValidator GetProofsRequestValidator { get; set; }

//    public void Be_Valid()
//    {
//      var __requestName__Request = new GetProofsRequest
//      {
//        // Set Valid values here
//        Days = 5
//      };

//      ValidationResult validationResult = GetProofsRequestValidator.TestValidate(__requestName__Request);

//      validationResult.IsValid.Should().BeTrue();
//    }

//    public void Have_error_when_Days_are_negative() => GetProofsRequestValidator
//      .ShouldHaveValidationErrorFor(aGetProofsRequest => aGetProofsRequest.Days, -1);

//    public void Setup() => GetProofsRequestValidator = new GetProofsRequestValidator();
//  }
//}
