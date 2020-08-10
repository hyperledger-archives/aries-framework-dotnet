namespace Hyperledger.Aries.AspNetCore.Features.PresentProofs
{
  using MediatR;
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using Hyperledger.Aries.Features.PresentProof;

  public class GetCredentialsForProofRequest : BaseApiRequest, IRequest<GetCredentialsForProofResponse>
  {
    public const string RouteTemplate = BaseRequest.BaseUri + "present-proof/records/{ProofId}/credentials";

    /// <summary>
    /// Set Properties and Update Docs
    /// </summary>
    /// <example>TODO</example>
    public string ProofId { get; set; } = null!;

    public string Referent { get; set; } = null!;

    internal override string GetRoute() => $"{RouteTemplate}?{nameof(ProofId)}={ProofId}&{nameof(CorrelationId)}={CorrelationId}";
  }
}