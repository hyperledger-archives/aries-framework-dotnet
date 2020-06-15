namespace BlazorHosted.Features.Wallets
{
  using MediatR;
  using BlazorHosted.Features.Bases;

  public class GetWalletRequest : BaseApiRequest, IRequest<GetWalletResponse>
  {
    public const string Route = "api/wallets/wallet";

    internal override string RouteFactory => $"{Route}?{nameof(CorrelationId)}={CorrelationId}";
  }
}