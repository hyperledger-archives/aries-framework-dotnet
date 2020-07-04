namespace Hyperledger.Aries.OpenApi.Features.PresentProofs
{
  using MediatR;
  using Hyperledger.Aries.OpenApi.Features.Bases;

  public class GetProofRequest : BaseApiRequest, IRequest<GetProofResponse>
  {
    public const string RouteTemplate = BaseRequest.BaseUri + "present-proof/records/{ProofId}";

    /// <summary>
    /// Set Properties and Update Docs
    /// </summary>
    /// <example>TODO</example>
    public string ProofId { get; set; } = null!;

    internal override string GetRoute() => $"{RouteTemplate}?{nameof(ProofId)}={ProofId}&{nameof(CorrelationId)}={CorrelationId}";
  }
}