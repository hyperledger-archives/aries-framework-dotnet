namespace BlazorHosted.Features.Wallets
{
  using FluentValidation;

  public class GetWalletRequestValidator : AbstractValidator<GetWalletRequest>
  {

    public GetWalletRequestValidator() { }
  }
}