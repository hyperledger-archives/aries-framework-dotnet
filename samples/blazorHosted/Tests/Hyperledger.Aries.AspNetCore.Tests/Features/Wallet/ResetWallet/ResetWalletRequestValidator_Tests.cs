namespace ResetWalletRequestValidator_
{
  using FluentAssertions;
  using FluentValidation.Results;
  using FluentValidation.TestHelper;
  using Hyperledger.Aries.AspNetCore.Features.Wallets;

  public class Validate_Should
  {
    private ResetWalletRequestValidator ResetWalletRequestValidator { get; set; }
    private ResetWalletRequest ResetWalletRequest { get; set; }

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
