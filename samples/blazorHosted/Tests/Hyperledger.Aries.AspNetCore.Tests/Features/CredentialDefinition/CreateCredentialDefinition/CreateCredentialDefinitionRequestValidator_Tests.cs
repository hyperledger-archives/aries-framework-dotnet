//namespace CreateCredentialDefinitionRequestValidator_
//{
//  using FluentAssertions;
//  using FluentValidation.Results;
//  using FluentValidation.TestHelper;
//  using Hyperledger.Aries.AspNetCore.Features.CredentialDefinitions;

//  public class Validate_Should
//  {
//    private CreateCredentialDefinitionRequestValidator CreateCredentialDefinitionRequestValidator { get; set; }

//    public void Be_Valid()
//    {
//      var __requestName__Request = new CreateCredentialDefinitionRequest
//      {
//        // Set Valid values here
//        Days = 5
//      };

//      ValidationResult validationResult = CreateCredentialDefinitionRequestValidator.TestValidate(__requestName__Request);

//      validationResult.IsValid.Should().BeTrue();
//    }

//    public void Have_error_when_Days_are_negative() => CreateCredentialDefinitionRequestValidator
//      .ShouldHaveValidationErrorFor(aCreateCredentialDefinitionRequest => aCreateCredentialDefinitionRequest.Days, -1);

//    public void Setup() => CreateCredentialDefinitionRequestValidator = new CreateCredentialDefinitionRequestValidator();
//  }
//}
