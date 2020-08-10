namespace Hyperledger.Aries.AspNetCore.Features.Credentials
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using MediatR;

  public class GetCredentialRequest : BaseApiRequest, IRequest<GetCredentialResponse>
  {
    public const string RouteTemplate = BaseRequest.BaseUri + "credentials/{CredentialId}";

    /// <summary>
    /// The Id of the Credential to return
    /// </summary>
    /// <example>5</example>
    public string CredentialId { get; set; } = null!;

    internal override string GetRoute()
    {
      string temp = RouteTemplate.Replace($"{{{nameof(CredentialId)}}}", CredentialId, System.StringComparison.Ordinal);
      return $"{temp}?{nameof(CorrelationId)}={CorrelationId}";
    }
  }
}
