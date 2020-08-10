namespace Hyperledger.Aries.AspNetCore.Features.Schemas
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using MediatR;

  public class GetSchemaRequest : BaseApiRequest, IRequest<GetSchemaResponse>
  {
    public const string RouteTemplate = BaseRequest.BaseUri + "schemas/{SchemaId}";

    /// <summary>
    /// Id of the Schema to return
    /// </summary>
    /// <example>5</example>
    public string SchemaId { get; set; } = null!;

    internal override string GetRoute()
    {
      string temp = RouteTemplate.Replace($"{{{nameof(SchemaId)}}}", SchemaId, System.StringComparison.Ordinal);
      return $"{temp}?{nameof(CorrelationId)}={CorrelationId}";
    }
  }
}
