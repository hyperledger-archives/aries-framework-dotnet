namespace ResetWalletRequestValidator_
{
  using FluentAssertions;
  using FluentValidation.Results;
  using FluentValidation.TestHelper;
  using BlazorHosted.Features.Wallets;

  public class Validate_Should
  {
    private ResetWalletRequestValidator ResetWalletRequestValidator { get; set; }
    public ResetWalletRequest ResetWalletRequest { get; private set; }

    public Validate_Should()
    {
      ResetWalletRequestValidator = new ResetWalletRequestValidator();
      ResetWalletRequest = new ResetWalletRequest();
    }

    public void Be_Valid()
    {
      ValidationResult validationResult = ResetWalletRequestValidator.TestValidate(ResetWalletRequest);

      validationResult.IsValid.Should().BeTrue();
    }
  }
}
