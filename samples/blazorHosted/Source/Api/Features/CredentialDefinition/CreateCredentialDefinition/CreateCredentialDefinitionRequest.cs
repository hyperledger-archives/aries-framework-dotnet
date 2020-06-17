namespace BlazorHosted.Features.CredentialDefinitions
{
  using MediatR;
  using BlazorHosted.Features.Bases;

  public class CreateCredentialDefinitionRequest : BaseApiRequest, IRequest<CreateCredentialDefinitionResponse>
  {
    public const string Route = "api/CredentialDefinitions/CreateCredentialDefinition";

    /// <summary>
    /// The Number of days of forecasts to get
    /// </summary>
    /// <example>5</example>
    public int Days { get; set; }

    internal override string RouteFactory => $"{Route}?{nameof(Days)}={Days}&{nameof(CorrelationId)}={CorrelationId}";
  }
}