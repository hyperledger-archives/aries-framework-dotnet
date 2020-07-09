//namespace GetHealthRequestValidator_
//{
//  using FluentAssertions;
//  using FluentValidation.Results;
//  using FluentValidation.TestHelper;
//  using Hyperledger.Aries.AspNetCore.Features.Healths;

//  public class Validate_Should
//  {
//    private GetHealthRequestValidator GetHealthRequestValidator { get; set; }

//    public void Be_Valid()
//    {
//      var __requestName__Request = new GetHealthRequest
//      {
//        // Set Valid values here
//        Days = 5
//      };

//      ValidationResult validationResult = GetHealthRequestValidator.TestValidate(__requestName__Request);

//      validationResult.IsValid.Should().BeTrue();
//    }

//    public void Have_error_when_Days_are_negative() => GetHealthRequestValidator
//      .ShouldHaveValidationErrorFor(aGetHealthRequest => aGetHealthRequest.Days, -1);

//    public void Setup() => GetHealthRequestValidator = new GetHealthRequestValidator();
//  }
//}
