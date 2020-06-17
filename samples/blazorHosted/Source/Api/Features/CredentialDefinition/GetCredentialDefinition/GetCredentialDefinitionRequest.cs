namespace BlazorHosted.Features.CredentialDefinitions
{
  using MediatR;
  using BlazorHosted.Features.Bases;

  public class GetCredentialDefinitionRequest : BaseApiRequest, IRequest<GetCredentialDefinitionResponse>
  {
    public const string Route = "api/CredentialDefinitions/GetCredentialDefinition";

    /// <summary>
    /// The Number of days of forecasts to get
    /// </summary>
    /// <example>5</example>
    public int Days { get; set; }

    internal override string RouteFactory => $"{Route}?{nameof(Days)}={Days}&{nameof(CorrelationId)}={CorrelationId}";
  }
}