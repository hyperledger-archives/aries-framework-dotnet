namespace BlazorHosted.Features.Wallets
{
  using MediatR;
  using BlazorHosted.Features.Bases;

  public class GetWalletRequest : BaseApiRequest, IRequest<GetWalletResponse>
  {
    public const string Route = "api/agent/provision";

    internal override string GetRoute() => $"{Route}?{nameof(CorrelationId)}={CorrelationId}";
  }
}