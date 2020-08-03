namespace GetWalletRequestValidator_
{
  using FluentAssertions;
  using FluentValidation.Results;
  using FluentValidation.TestHelper;
  using Hyperledger.Aries.AspNetCore.Features.Wallets;

  public class Validate_Should
  {
    private GetWalletRequestValidator GetWalletRequestValidator { get; set; }
    public GetWalletRequest GetWalletRequest { get; private set; }

    public Validate_Should()
    {
      GetWalletRequestValidator = new GetWalletRequestValidator();
      GetWalletRequest = new GetWalletRequest();
    }

    public void Be_Valid()
    {
      ValidationResult validationResult = GetWalletRequestValidator.TestValidate(GetWalletRequest);

      validationResult.IsValid.Should().BeTrue();
    }

  }
}
