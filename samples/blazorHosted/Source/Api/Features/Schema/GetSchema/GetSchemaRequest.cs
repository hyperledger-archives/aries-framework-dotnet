namespace BlazorHosted.Features.Schemas
{
  using BlazorHosted.Features.Bases;
  using MediatR;

  public class GetSchemaRequest : BaseApiRequest, IRequest<GetSchemaResponse>
  {
    public const string Route = "api/schemas/{SchemaId}";

    /// <summary>
    /// Id of the Schema to return
    /// </summary>
    /// <example>5</example>
    public string SchemaId { get; set; } = null!;

    internal override string RouteFactory
    {
      get
      {
        string temp = Route.Replace($"{{{nameof(SchemaId)}}}", SchemaId, System.StringComparison.Ordinal);
        return $"{temp}?{nameof(CorrelationId)}={CorrelationId}";
      }
    }
  }
}
