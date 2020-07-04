namespace Hyperledger.Aries.OpenApi.Features.Wallets
{
  using Hyperledger.Aries.OpenApi.Features.Bases;
  using MediatR;

  public class ResetWalletRequest : BaseApiRequest, IRequest<ResetWalletResponse>
  {
    public const string RouteTemplate = BaseRequest.BaseUri + "agent/reset";

    internal override string GetRoute() => $"{RouteTemplate}?{nameof(CorrelationId)}={CorrelationId}";
  }
}
