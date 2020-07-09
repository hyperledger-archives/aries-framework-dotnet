namespace Hyperledger.Aries.AspNetCore.Features.BasicMessaging
{
  using MediatR;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public class SendMessageRequest : BaseApiRequest, IRequest<SendMessageResponse>
  {
    public const string RouteTemplate = BaseRequest.BaseUri + "connections/{ConnectionId}/send-message";

    /// <summary>
    /// The Id of the Connection to use to send the message
    /// </summary>
    /// <example>Connection identifier</example>
    public string ConnectionId { get; set; } = null!;

    /// <summary>
    /// The Message to send
    /// </summary>
    /// <example>Hello Friend</example>
    public string Message { get; set; } = null!;

    internal override string GetRoute() =>
      RouteTemplate.Replace($"{{{nameof(ConnectionId)}}}", ConnectionId, System.StringComparison.Ordinal);
  }
}