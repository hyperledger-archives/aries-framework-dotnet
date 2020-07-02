namespace BlazorHosted.Features.Connections
{
  using BlazorHosted.Features.Bases;
  using Dawn;
  using MediatR;

  public class DeleteConnectionRequest : BaseApiRequest, IRequest<DeleteConnectionResponse>
  {
    public const string RouteTemplate = "api/connections/{ConnectionId}";

    /// <summary>
    /// The Id of the Connection to use to send the message
    /// </summary>
    /// <example>Connection identifier</example>
    public string ConnectionId { get; set; } = null!;

    public DeleteConnectionRequest() { }

    public DeleteConnectionRequest(string aConnectionId)
    {
      ConnectionId = aConnectionId;
    }

    internal override string GetRoute()
    {
      ConnectionId = Guard.Argument(ConnectionId, nameof(ConnectionId)).NotNull().NotEmpty();
      string temp = RouteTemplate.Replace($"{{{nameof(ConnectionId)}}}", ConnectionId, System.StringComparison.Ordinal);
      return $"{temp}?{nameof(CorrelationId)}={CorrelationId}";
    }
  }
}
