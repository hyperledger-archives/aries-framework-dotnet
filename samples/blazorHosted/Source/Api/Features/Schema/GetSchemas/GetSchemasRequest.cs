namespace BlazorHosted.Features.Schemas
{
  using MediatR;
  using BlazorHosted.Features.Bases;

  public class GetSchemasRequest : BaseApiRequest, IRequest<GetSchemasResponse>
  {
    // Trinsic API Route /definitions/schemas
    public const string RouteTemplate = BaseRequest.BaseUri + "schemas";

    internal override string GetRoute() => $"{RouteTemplate}?{nameof(CorrelationId)}={CorrelationId}";
  }
}