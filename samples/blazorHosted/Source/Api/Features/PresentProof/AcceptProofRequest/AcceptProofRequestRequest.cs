namespace BlazorHosted.Features.PresentProofs
{
  using MediatR;
  using BlazorHosted.Features.Bases;

  public class AcceptProofRequestRequest : BaseApiRequest, IRequest<AcceptProofRequestResponse>
  {
    public const string RouteTemplate = "api/present-proof/accept";

    /// <summary>
    /// Set Properties and Update Docs
    /// </summary>
    /// <example>TODO</example>
    public string EncodedProofRequestMessage { get; set; } = null!;

    internal override string GetRoute() => RouteTemplate;
  }
}