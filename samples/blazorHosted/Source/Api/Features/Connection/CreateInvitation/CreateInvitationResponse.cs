namespace BlazorHosted.Features.Connections
{
  using BlazorHosted.Features.Bases;
  using Hyperledger.Aries.Features.DidExchange;
  using System;
  using System.Collections.Generic;

  public class CreateInvitationResponse : BaseResponse
  {
    /// <summary>
    /// Represents an invitation message for establishing connection.
    /// </summary>
    public ConnectionInvitationMessage ConnectionInvitationMessage { get; set; } = null!;

    public string InvitationUrl { get; set; } = null!;


    public CreateInvitationResponse() { }

    public CreateInvitationResponse(Guid aRequestId, ConnectionInvitationMessage aConnectionInvitationMessage) : base(aRequestId)
    {
      ConnectionInvitationMessage = aConnectionInvitationMessage;
    }
  }

  /// <summary>
  /// Represents an invitation message for establishing connection.
  /// </summary>
  public class InvitationDto
  {
    public Guid InvitationId { get; set; }
    /// <summary>
    /// Gets or sets the image URL.
    /// </summary>
    /// <value>
    /// The image URL.
    /// </value>
    public Uri? ImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Label { get; set; } = null!;

    /// <summary>
    /// Gets or sets the recipient keys.
    /// </summary>
    /// <value>
    /// The recipient keys.
    /// </value>
    public List<string> RecipientKeys { get; set; } = null!;

    /// <summary>
    /// Gets or sets the routing keys.
    /// </summary>
    /// <value>
    /// The routing keys.
    /// </value>
    public List<string> RoutingKeys { get; set; } = null!;

    /// <summary>
    /// Gets or sets the service endpoint.
    /// </summary>
    /// <value>
    /// The service endpoint.
    /// </value>
    public string ServiceEndpoint { get; set; } = null!;

    public InvitationDto() { }

    public InvitationDto
    (
      string aLabel,
      Uri? aImageUrl,
      string aServiceEndpoint,
      IList<string> aRoutingKeys,
      IList<string> aRecipientKeys
    )
    {
      Label = aLabel;
      ImageUrl = aImageUrl;
      ServiceEndpoint = aServiceEndpoint;
      RoutingKeys = new List<string>(aRoutingKeys);
      RecipientKeys = new List<string>(aRecipientKeys);
    }
  }
}
