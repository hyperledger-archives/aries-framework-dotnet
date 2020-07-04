namespace Hyperledger.Aries.OpenApi.Features.Connections
{
  using Hyperledger.Aries.OpenApi.Features.Bases;
  using MediatR;

  public class GetConnectionsRequest : BaseApiRequest, IRequest<GetConnectionsResponse>
  {
    public const string RouteTemplate = BaseRequest.BaseUri + "connections";

    internal override string GetRoute() => $"{RouteTemplate}?{nameof(CorrelationId)}={CorrelationId}";
  }
}
