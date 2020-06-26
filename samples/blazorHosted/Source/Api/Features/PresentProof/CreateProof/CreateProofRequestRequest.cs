namespace BlazorHosted.Features.PresentProofs
{
  using MediatR;
  using BlazorHosted.Features.Bases;
  using Hyperledger.Aries.Features.PresentProof;

  public class CreateProofRequestRequest : BaseApiRequest, IRequest<CreateProofRequestRequestResponse>
  {
    public const string RouteTemplate = "api/present-proof/send-request";

    /// <summary>
    /// Set Properties and Update Docs
    /// </summary>
    /// <example>TODO</example>
    public string ConnectionId { get; set; } = null!;

    public string? Comment { get; set; }

    public bool Trace { get; set; } = false;

    public ProofRequest ProofRequest { get; set; } = null!;

    internal override string GetRoute() => RouteTemplate;
  }
}