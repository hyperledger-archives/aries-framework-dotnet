namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using MediatR;
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using Dawn;

  public class GetConnectionRequest : BaseApiRequest, IRequest<GetConnectionResponse>
  {
    public const string RouteTemplate = BaseRequest.BaseUri + "connections/{ConnectionId}";

    /// <summary>
    /// The Id of the Connection
    /// </summary>
    /// <example>Connection identifier</example>
    public string ConnectionId { get; set; } = null!;

    internal override string GetRoute()
    {
      ConnectionId = Guard.Argument(ConnectionId, nameof(ConnectionId)).NotNull().NotEmpty();

      string temp = RouteTemplate.Replace($"{{{nameof(ConnectionId)}}}", ConnectionId, System.StringComparison.Ordinal);
      return $"{temp}?{nameof(CorrelationId)}={CorrelationId}";
    }

    public GetConnectionRequest() { }
    public GetConnectionRequest(string aConnectionId)
    {
      ConnectionId = aConnectionId;
    }
  }
}