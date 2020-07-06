//namespace GetSchemaRequestValidator_
//{
//  using FluentAssertions;
//  using FluentValidation.Results;
//  using FluentValidation.TestHelper;
//  using Hyperledger.Aries.AspNetCore.Features.Schemas;

//  public class Validate_Should
//  {
//    private GetSchemaRequestValidator GetSchemaRequestValidator { get; set; }

//    public void Be_Valid()
//    {
//      var __requestName__Request = new GetSchemaRequest
//      {
//        // Set Valid values here
//        Days = 5
//      };

//      ValidationResult validationResult = GetSchemaRequestValidator.TestValidate(__requestName__Request);

//      validationResult.IsValid.Should().BeTrue();
//    }

//    public void Have_error_when_Days_are_negative() => GetSchemaRequestValidator
//      .ShouldHaveValidationErrorFor(aGetSchemaRequest => aGetSchemaRequest.Days, -1);

//    public void Setup() => GetSchemaRequestValidator = new GetSchemaRequestValidator();
//  }
//}
