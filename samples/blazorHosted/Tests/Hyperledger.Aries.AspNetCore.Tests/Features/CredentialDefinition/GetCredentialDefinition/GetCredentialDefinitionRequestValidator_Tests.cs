//namespace GetCredentialDefinitionRequestValidator_
//{
//  using FluentAssertions;
//  using FluentValidation.Results;
//  using FluentValidation.TestHelper;
//  using Hyperledger.Aries.AspNetCore.Features.CredentialDefinitions;

//  public class Validate_Should
//  {
//    private GetCredentialDefinitionRequestValidator GetCredentialDefinitionRequestValidator { get; set; }

//    public void Be_Valid()
//    {
//      var __requestName__Request = new GetCredentialDefinitionRequest
//      {
//        // Set Valid values here
//        Days = 5
//      };

//      ValidationResult validationResult = GetCredentialDefinitionRequestValidator.TestValidate(__requestName__Request);

//      validationResult.IsValid.Should().BeTrue();
//    }

//    public void Have_error_when_Days_are_negative() => GetCredentialDefinitionRequestValidator
//      .ShouldHaveValidationErrorFor(aGetCredentialDefinitionRequest => aGetCredentialDefinitionRequest.Days, -1);

//    public void Setup() => GetCredentialDefinitionRequestValidator = new GetCredentialDefinitionRequestValidator();
//  }
//}
