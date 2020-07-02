namespace BlazorHosted.Features.PresentProofs
{
  using MediatR;
  using BlazorHosted.Features.Bases;

  public class GetProofsRequest : BaseApiRequest, IRequest<GetProofsResponse>
  {
    public const string RouteTemplate = BaseRequest.BaseUri + "present-proof/records";
    internal override string GetRoute() => $"{RouteTemplate}?{nameof(CorrelationId)}={CorrelationId}";
  }
}