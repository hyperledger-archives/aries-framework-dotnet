namespace BlazorHosted.Features.Connections
{
  using MediatR;
  using BlazorHosted.Features.Bases;

  public class AcceptInvitationRequest : BaseApiRequest, IRequest<AcceptInvitationResponse>
  {
    public const string RouteTemplate = "api/connections/accept-invitation";

    public string InvitationDetails { get; set; } = null!;

    internal override string GetRoute() => $"{RouteTemplate}?{nameof(CorrelationId)}={CorrelationId}";
  }
}