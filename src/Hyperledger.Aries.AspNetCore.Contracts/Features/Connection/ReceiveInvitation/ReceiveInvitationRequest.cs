namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using MediatR;

  public class ReceiveInvitationRequest : BaseApiRequest, IRequest<ReceiveInvitationResponse>
  {
    public const string RouteTemplate = BaseRequest.BaseUri + "connections/receive-invitation";

    public string InvitationDetails { get; set; } = null!;

    internal override string GetRoute() => $"{RouteTemplate}?{nameof(CorrelationId)}={CorrelationId}";

    public ReceiveInvitationRequest(string aInvitationDetails)
    {
      InvitationDetails = aInvitationDetails;
    }
  }
}
