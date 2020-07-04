namespace Hyperledger.Aries.OpenApi.Features.Wallets
{
  using MediatR;
  using Hyperledger.Aries.OpenApi.Features.Bases;

  public class GetWalletRequest : BaseApiRequest, IRequest<GetWalletResponse>
  {
    public const string RouteTemplate = BaseRequest.BaseUri + "agent/provision";

    internal override string GetRoute() => $"{RouteTemplate}?{nameof(CorrelationId)}={CorrelationId}";
  }
}