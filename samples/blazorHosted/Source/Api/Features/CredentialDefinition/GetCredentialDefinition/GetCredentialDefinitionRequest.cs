namespace BlazorHosted.Features.CredentialDefinitions
{
  using MediatR;
  using BlazorHosted.Features.Bases;

  public class GetCredentialDefinitionRequest : BaseApiRequest, IRequest<GetCredentialDefinitionResponse>
  {
    public const string Route = "api/credential-definitions/{CredentialDefinitionId}";

    /// <summary>
    /// The Id of the Credential Definition to retrieve
    /// </summary>
    /// <example>5</example>
    public string CredentialDefinitionId { get; set; } = null!;

    public GetCredentialDefinitionRequest() { }
    public GetCredentialDefinitionRequest(string aCredentialDefinitionId)
    {
      CredentialDefinitionId = aCredentialDefinitionId;
    }

    internal override string GetRoute()
    {
      string temp =
        Route
        .Replace($"{{{nameof(CredentialDefinitionId)}}}", CredentialDefinitionId, System.StringComparison.Ordinal);

      return $"{temp}?{nameof(CorrelationId)}={CorrelationId}";
    }
  }
}