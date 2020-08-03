namespace Hyperledger.Aries.AspNetCore.Features.PresentProofs
{
  using MediatR;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public class AcceptProofRequestRequest : BaseApiRequest, IRequest<AcceptProofRequestResponse>
  {
    public const string RouteTemplate = BaseRequest.BaseUri + "present-proof/accept";

    /// <summary>
    /// Set Properties and Update Docs
    /// </summary>
    /// <example>TODO</example>
    public string EncodedProofRequestMessage { get; set; } = null!;

    internal override string GetRoute() => RouteTemplate;
  }
}