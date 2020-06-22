namespace BlazorHosted.Features.Connections
{
  using BlazorHosted.Features.Bases;
  using MediatR;

  public class RecieveInvitationRequest : BaseApiRequest, IRequest<RecieveInvitationResponse>
  {
    public const string RouteTemplate = "api/connections/recieve-invitation";

    public string InvitationDetails { get; set; } = null!;

    internal override string GetRoute() => $"{RouteTemplate}?{nameof(CorrelationId)}={CorrelationId}";
  }
}
