namespace BlazorHosted.Features.PresentProofs
{
  using MediatR;
  using BlazorHosted.Features.Bases;

  public class GetProofsRequest : BaseApiRequest, IRequest<GetProofsResponse>
  {
    public const string RouteTemplate = "api/present-proof/records";

    /// <summary>
    /// Set Properties and Update Docs
    /// </summary>
    /// <example>TODO</example>
    public string SampleProperty { get; set; } = null!;

    internal override string GetRoute() => $"{RouteTemplate}?{nameof(SampleProperty)}={SampleProperty}&{nameof(CorrelationId)}={CorrelationId}";
  }
}