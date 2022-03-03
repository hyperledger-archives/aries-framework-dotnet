namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using Aries.Features.Handshakes.Connection.Models;
  using Bases;
  using MediatR;

  /// <summary>
  /// Create Invitation Request
  /// </summary>
  public class CreateInvitationRequest : BaseApiRequest, IRequest<CreateInvitationResponse>
  {
    public const string RouteTemplate = BaseRequest.BaseUri + "connections/create-invitation";

    public InviteConfiguration InviteConfiguration { get; set; } = null!;

    /// <summary>
    /// Parameterless constructor for System.Text.Json
    /// </summary>
    public CreateInvitationRequest() { }

    public CreateInvitationRequest(InviteConfiguration aInviteConfiguration)
    {
      InviteConfiguration = aInviteConfiguration;
    }

    internal override string GetRoute() => RouteTemplate;
  }
}
