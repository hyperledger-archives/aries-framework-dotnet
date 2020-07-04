namespace Hyperledger.Aries.OpenApi.Features.Healths
{
  using Hyperledger.Aries.OpenApi.Features.Bases;
  using MediatR;

  public class GetHealthRequest : BaseApiRequest, IRequest<GetHealthResponse>
  {
    public const string RouteTemplate = BaseRequest.BaseUri + "Health/GetHealth";

    /// <summary>
    /// The Number of days of forecasts to get
    /// </summary>
    /// <example>5</example>
    public int Days { get; set; }

    internal override string GetRoute() => $"{RouteTemplate}?{nameof(Days)}={Days}&{nameof(CorrelationId)}={CorrelationId}";
  }
}
