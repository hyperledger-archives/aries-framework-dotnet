//namespace CreateSchemaRequestValidator_
//{
//  using FluentAssertions;
//  using FluentValidation.Results;
//  using FluentValidation.TestHelper;
//  using Hyperledger.Aries.AspNetCore.Features.Schemas;

//  public class Validate_Should
//  {
//    private CreateSchemaRequestValidator CreateSchemaRequestValidator { get; set; }

//    public void Be_Valid()
//    {
//      var __requestName__Request = new CreateSchemaRequest
//      {
//        // Set Valid values here
//        Days = 5
//      };

//      ValidationResult validationResult = CreateSchemaRequestValidator.TestValidate(__requestName__Request);

//      validationResult.IsValid.Should().BeTrue();
//    }

//    public void Have_error_when_Days_are_negative() => CreateSchemaRequestValidator
//      .ShouldHaveValidationErrorFor(aCreateSchemaRequest => aCreateSchemaRequest.Days, -1);

//    public void Setup() => CreateSchemaRequestValidator = new CreateSchemaRequestValidator();
//  }
//}
