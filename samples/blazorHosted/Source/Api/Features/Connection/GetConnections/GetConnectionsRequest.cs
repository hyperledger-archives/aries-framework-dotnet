namespace BlazorHosted.Features.Connections
{
  using MediatR;
  using BlazorHosted.Features.Bases;

  public class GetConnectionsRequest : BaseApiRequest, IRequest<GetConnectionsResponse>
  {
    public const string Route = "api/connections";

    internal override string RouteFactory => $"{Route}?{nameof(CorrelationId)}={CorrelationId}";
  }
}