//namespace GetCredentialsForProofRequestValidator_
//{
//  using FluentAssertions;
//  using FluentValidation.Results;
//  using FluentValidation.TestHelper;
//  using Hyperledger.Aries.AspNetCore.Features.PresentProofs;

//  public class Validate_Should
//  {
//    private GetCredentialsForProofRequestValidator GetCredentialsForProofRequestValidator { get; set; }

//    public void Be_Valid()
//    {
//      var __requestName__Request = new GetCredentialsForProofRequest
//      {
//        // Set Valid values here
//        Days = 5
//      };

//      ValidationResult validationResult = GetCredentialsForProofRequestValidator.TestValidate(__requestName__Request);

//      validationResult.IsValid.Should().BeTrue();
//    }

//    public void Have_error_when_Days_are_negative() => GetCredentialsForProofRequestValidator
//      .ShouldHaveValidationErrorFor(aGetCredentialsForProofRequest => aGetCredentialsForProofRequest.Days, -1);

//    public void Setup() => GetCredentialsForProofRequestValidator = new GetCredentialsForProofRequestValidator();
//  }
//}
