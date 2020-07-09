//namespace GetCredentialsRequestValidator_
//{
//  using FluentAssertions;
//  using FluentValidation.Results;
//  using FluentValidation.TestHelper;
//  using Hyperledger.Aries.AspNetCore.Features.Credentials;

//  public class Validate_Should
//  {
//    private GetCredentialsRequestValidator GetCredentialsRequestValidator { get; set; }

//    public void Be_Valid()
//    {
//      var __requestName__Request = new GetCredentialsRequest
//      {
//        // Set Valid values here
//        Days = 5
//      };

//      ValidationResult validationResult = GetCredentialsRequestValidator.TestValidate(__requestName__Request);

//      validationResult.IsValid.Should().BeTrue();
//    }

//    public void Have_error_when_Days_are_negative() => GetCredentialsRequestValidator
//      .ShouldHaveValidationErrorFor(aGetCredentialsRequest => aGetCredentialsRequest.Days, -1);

//    public void Setup() => GetCredentialsRequestValidator = new GetCredentialsRequestValidator();
//  }
//}
