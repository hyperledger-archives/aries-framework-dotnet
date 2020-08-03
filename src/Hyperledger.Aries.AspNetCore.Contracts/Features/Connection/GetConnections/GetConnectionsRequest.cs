namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using MediatR;

  public class GetConnectionsRequest : BaseApiRequest, IRequest<GetConnectionsResponse>
  {
    public const string RouteTemplate = BaseRequest.BaseUri + "connections";

    internal override string GetRoute() => $"{RouteTemplate}?{nameof(CorrelationId)}={CorrelationId}";
  }
}
