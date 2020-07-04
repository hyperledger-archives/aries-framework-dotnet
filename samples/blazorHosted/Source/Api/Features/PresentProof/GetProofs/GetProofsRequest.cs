namespace Hyperledger.Aries.OpenApi.Features.PresentProofs
{
  using MediatR;
  using Hyperledger.Aries.OpenApi.Features.Bases;

  public class GetProofsRequest : BaseApiRequest, IRequest<GetProofsResponse>
  {
    public const string RouteTemplate = BaseRequest.BaseUri + "present-proof/records";
    internal override string GetRoute() => $"{RouteTemplate}?{nameof(CorrelationId)}={CorrelationId}";
  }
}