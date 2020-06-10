namespace BlazorHosted.Features.Connections
{
  using MediatR;
  using BlazorHosted.Features.Bases;

  public class CreateInvitationRequest : BaseApiRequest, IRequest<CreateInvitationResponse>
  {
    public const string Route = "api/Connections/CreateInvitation";

    /// <summary>
    /// The Number of days of forecasts to get
    /// </summary>
    /// <example>5</example>
    public int Days { get; set; }

    internal override string RouteFactory => Route;
  }
}