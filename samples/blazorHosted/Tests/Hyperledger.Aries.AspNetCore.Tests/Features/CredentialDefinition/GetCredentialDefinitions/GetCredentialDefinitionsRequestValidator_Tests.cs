//namespace GetCredentialDefinitionsRequestValidator_
//{
//  using FluentAssertions;
//  using FluentValidation.Results;
//  using FluentValidation.TestHelper;
//  using Hyperledger.Aries.AspNetCore.Features.CredentialDefinitions;

//  public class Validate_Should
//  {
//    private GetCredentialDefinitionsRequestValidator GetCredentialDefinitionsRequestValidator { get; set; }

//    public void Be_Valid()
//    {
//      var __requestName__Request = new GetCredentialDefinitionsRequest
//      {
//        // Set Valid values here
//        Days = 5
//      };

//      ValidationResult validationResult = GetCredentialDefinitionsRequestValidator.TestValidate(__requestName__Request);

//      validationResult.IsValid.Should().BeTrue();
//    }

//    public void Have_error_when_Days_are_negative() => GetCredentialDefinitionsRequestValidator
//      .ShouldHaveValidationErrorFor(aGetCredentialDefinitionsRequest => aGetCredentialDefinitionsRequest.Days, -1);

//    public void Setup() => GetCredentialDefinitionsRequestValidator = new GetCredentialDefinitionsRequestValidator();
//  }
//}
