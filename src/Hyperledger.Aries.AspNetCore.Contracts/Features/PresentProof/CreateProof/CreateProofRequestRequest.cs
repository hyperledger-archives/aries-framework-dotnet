namespace Hyperledger.Aries.AspNetCore.Features.PresentProofs
{
  using MediatR;
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using Hyperledger.Aries.Features.PresentProof;

  public class CreateProofRequestRequest : BaseApiRequest, IRequest<CreateProofRequestResponse>
  {
    public const string RouteTemplate = BaseRequest.BaseUri + "present-proof/create-request";

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