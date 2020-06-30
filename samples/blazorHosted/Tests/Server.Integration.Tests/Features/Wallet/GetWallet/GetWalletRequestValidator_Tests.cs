namespace GetWalletRequestValidator_
{
  using FluentAssertions;
  using FluentValidation.Results;
  using FluentValidation.TestHelper;
  using BlazorHosted.Features.Wallets;

  public class Validate_Should
  {
    private GetWalletRequestValidator GetWalletRequestValidator { get; set; }

    public Validate_Should()
    {
      GetWalletRequestValidator = new GetWalletRequestValidator();
    }

    public void Be_Valid()
    {
      var getWalletRequest = new GetWalletRequest
      {
        // Set Valid values here
        CorrelationId = System.Guid.NewGuid()
      };

      ValidationResult validationResult = GetWalletRequestValidator.TestValidate(getWalletRequest);

      validationResult.IsValid.Should().BeTrue();
    }

  }
}
