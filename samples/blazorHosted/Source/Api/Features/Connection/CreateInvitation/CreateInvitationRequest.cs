namespace BlazorHosted.Features.Connections
{
  using MediatR;
  using BlazorHosted.Features.Bases;
  using System.Collections.Generic;
  using Hyperledger.Aries.Features.DidExchange;


  /// <summary>
  /// Create Invitation Request
  /// </summary>
  public class CreateInvitationRequest : BaseApiRequest, IRequest<CreateInvitationResponse>
  {
    public const string Route = "api/connections/create-invitation";

    public InviteConfiguration InviteConfiguration { get; set; }

    /// <summary>
    /// Used to generated an invitation that multiple parties
    /// can use to connect.
    /// </summary>
    public bool MultiPartyInvitation { get; set; }


    /// <summary>
    /// Gets or sets the name of the alias for the connection.
    /// </summary>
    /// <value>
    /// The name of the alias for the connection.
    /// </value>
    /// <example>Alice</example>
    public string? Alias { get; set; }

    /// <summary>
    /// Gets or sets the url of an image of the alias for the connection.
    /// </summary>
    /// <value>
    /// The image url of the alias for the connection.
    /// </value>
    /// <example>https://www.gravatar.com/avatar/fb214494d2a75080e8019f5fc961a1d9</example>
    public System.Uri? ImageUrl { get; set; }

    /// <summary>
    /// For automatically accepting a
    /// connection request generated from this created invite
    /// </summary>
    public bool AutoAcceptConnection { get; set; }

    /// <summary>
    /// Controls the tags that are persisted against the invite/connection record.
    /// </summary>
    public Dictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();

    public bool Public { get; set; }

    public CreateInvitationRequest()
    {
      InviteConfiguration = new InviteConfiguration { AutoAcceptConnection = true };
    }
    internal override string GetRoute() => Route;
  }

}