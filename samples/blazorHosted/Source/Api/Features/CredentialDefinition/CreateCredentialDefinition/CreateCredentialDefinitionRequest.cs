namespace BlazorHosted.Features.CredentialDefinitions
{
  using MediatR;
  using BlazorHosted.Features.Bases;

  public class CreateCredentialDefinitionRequest : BaseApiRequest, IRequest<CreateCredentialDefinitionResponse>
  {
    public const string Route = "api/CredentialDefinitions/CreateCredentialDefinition";

    public string SchemaId { get; set; } = null!;
    public string? Tag { get; set; }
    public bool EnableRevocation { get; set; } = false;
    public string RevocationRegistryBaseUri { get; set; } = string.Empty;
    public int RevocationRegistrySize { get; set; }
    public bool RevocationRegistryAutoScale { get; set; }

    /// <summary>
    /// The Number of days of forecasts to get
    /// </summary>
    /// <example>5</example>
    public int Days { get; set; }

    internal override string RouteFactory => $"{Route}?{nameof(Days)}={Days}&{nameof(CorrelationId)}={CorrelationId}";
  }
}