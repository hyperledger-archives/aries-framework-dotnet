namespace BlazorHosted.Features.Connections
{
  using MediatR;
  using BlazorHosted.Features.Bases;

  public class SendPingRequest : BaseApiRequest, IRequest<SendPingResponse>
  {
    public const string RouteTemplate = "api/connections/{ConnectionId}/send-ping";

    /// <summary>
    /// The Id of the Connection to use to send the message
    /// </summary>
    /// <example>Connection identifier</example>
    public string ConnectionId { get; set; } = null!;

    internal override string GetRoute() =>
      RouteTemplate.Replace($"{{{nameof(ConnectionId)}}}", ConnectionId, System.StringComparison.Ordinal);
  }
}