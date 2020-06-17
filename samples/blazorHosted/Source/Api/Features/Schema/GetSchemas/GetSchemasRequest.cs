namespace BlazorHosted.Features.Schemas
{
  using MediatR;
  using BlazorHosted.Features.Bases;

  public class GetSchemasRequest : BaseApiRequest, IRequest<GetSchemasResponse>
  {
    public const string Route = "api/schemas";

    internal override string RouteFactory => $"{Route}?{nameof(CorrelationId)}={CorrelationId}";
  }
}