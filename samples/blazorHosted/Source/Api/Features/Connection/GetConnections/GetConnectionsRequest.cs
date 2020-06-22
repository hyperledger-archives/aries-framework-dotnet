namespace BlazorHosted.Features.Connections
{
  using BlazorHosted.Features.Bases;
  using MediatR;

  public class GetConnectionsRequest : BaseApiRequest, IRequest<GetConnectionsResponse>
  {
    public const string RouteTemplate = "api/connections";

    internal override string GetRoute() => $"{RouteTemplate}?{nameof(CorrelationId)}={CorrelationId}";
  }
}
