namespace BlazorHosted.Features.Credentials
{
  using MediatR;
  using BlazorHosted.Features.Bases;

  public class RemoveCredentialRequest : BaseApiRequest, IRequest<RemoveCredentialResponse>
  {
    public const string Route = "api/credentials/{CredentialId}/remove";

    /// <summary>
    /// The Id of the Credential to return
    /// </summary>
    /// <example>5</example>
    public string CredentialId { get; set; } = null!;

    internal override string RouteFactory
    {
      get
      {
        string temp = Route.Replace($"{{{nameof(CredentialId)}}}", CredentialId, System.StringComparison.Ordinal);
        return $"{temp}?{nameof(CorrelationId)}={CorrelationId}";
      }
    }
  }
}