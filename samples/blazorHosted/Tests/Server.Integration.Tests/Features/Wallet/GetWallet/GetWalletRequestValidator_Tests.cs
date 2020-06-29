//namespace GetWalletRequestValidator_
//{
//  using FluentAssertions;
//  using FluentValidation.Results;
//  using FluentValidation.TestHelper;
//  using BlazorHosted.Features.Wallets;

//  public class Validate_Should
//  {
//    private GetWalletRequestValidator GetWalletRequestValidator { get; set; }

//    public void Be_Valid()
//    {
//      var __requestName__Request = new GetWalletRequest
//      {
//        // Set Valid values here
//        Days = 5
//      };

//      ValidationResult validationResult = GetWalletRequestValidator.TestValidate(__requestName__Request);

//      validationResult.IsValid.Should().BeTrue();
//    }

//    public void Have_error_when_Days_are_negative() => GetWalletRequestValidator
//      .ShouldHaveValidationErrorFor(aGetWalletRequest => aGetWalletRequest.Days, -1);

//    public void Setup() => GetWalletRequestValidator = new GetWalletRequestValidator();
//  }
//}
