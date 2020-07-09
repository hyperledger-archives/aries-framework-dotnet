//namespace GetSchemasRequestValidator_
//{
//  using FluentAssertions;
//  using FluentValidation.Results;
//  using FluentValidation.TestHelper;
//  using Hyperledger.Aries.AspNetCore.Features.Schemas;

//  public class Validate_Should
//  {
//    private GetSchemasRequestValidator GetSchemasRequestValidator { get; set; }

//    public void Be_Valid()
//    {
//      var __requestName__Request = new GetSchemasRequest
//      {
//        // Set Valid values here
//        Days = 5
//      };

//      ValidationResult validationResult = GetSchemasRequestValidator.TestValidate(__requestName__Request);

//      validationResult.IsValid.Should().BeTrue();
//    }

//    public void Have_error_when_Days_are_negative() => GetSchemasRequestValidator
//      .ShouldHaveValidationErrorFor(aGetSchemasRequest => aGetSchemasRequest.Days, -1);

//    public void Setup() => GetSchemasRequestValidator = new GetSchemasRequestValidator();
//  }
//}
