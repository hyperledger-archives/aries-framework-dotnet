namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using MediatR;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public class AcceptInvitationRequest : BaseApiRequest, IRequest<AcceptInvitationResponse>
  {
    public const string RouteTemplate = BaseRequest.BaseUri + "connections/accept-invitation";

    public string InvitationDetails { get; set; } = null!;

    internal override string GetRoute() => $"{RouteTemplate}?{nameof(CorrelationId)}={CorrelationId}";
  }
}