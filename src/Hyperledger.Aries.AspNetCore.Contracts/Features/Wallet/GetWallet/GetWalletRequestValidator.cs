namespace Hyperledger.Aries.AspNetCore.Features.Wallets
{
  using FluentValidation;

  public class GetWalletRequestValidator : AbstractValidator<GetWalletRequest>
  {

    public GetWalletRequestValidator() { }
  }
}