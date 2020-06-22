namespace BlazorHosted.Features.Credentials
{
  using BlazorHosted.Features.Bases;
  using MediatR;

  public class GetCredentialRequest : BaseApiRequest, IRequest<GetCredentialResponse>
  {
    public const string Route = "api/credentials/{CredentialId}";

    /// <summary>
    /// The Id of the Credential to return
    /// </summary>
    /// <example>5</example>
    public string CredentialId { get; set; } = null!;

    internal override string GetRoute()
    {
      string temp = Route.Replace($"{{{nameof(CredentialId)}}}", CredentialId, System.StringComparison.Ordinal);
      return $"{temp}?{nameof(CorrelationId)}={CorrelationId}";
    }
  }
}
