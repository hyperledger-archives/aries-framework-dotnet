namespace Hyperledger.Aries.AspNetCore.Features.Schemas
{
  using MediatR;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public class GetSchemasRequest : BaseApiRequest, IRequest<GetSchemasResponse>
  {
    // Trinsic API Route /definitions/schemas
    public const string RouteTemplate = BaseRequest.BaseUri + "schemas";

    internal override string GetRoute() => $"{RouteTemplate}?{nameof(CorrelationId)}={CorrelationId}";
  }
}