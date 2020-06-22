namespace BlazorHosted.Features.CredentialDefinitions
{
  using BlazorHosted.Features.Bases;
  using MediatR;

  public class GetCredentialDefinitionsRequest : BaseApiRequest, IRequest<GetCredentialDefinitionsResponse>
  {
    public const string Route = "api/credential-definitions";

    internal override string GetRoute() => $"{Route}?{nameof(CorrelationId)}={CorrelationId}";
  }
}
