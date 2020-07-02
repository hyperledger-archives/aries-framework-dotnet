namespace BlazorHosted.Features.CredentialDefinitions
{
  using BlazorHosted.Features.Bases;
  using MediatR;

  public class GetCredentialDefinitionsRequest : BaseApiRequest, IRequest<GetCredentialDefinitionsResponse>
  {
    public const string RouteTemplate = BaseRequest.BaseUri + "credential-definitions";

    internal override string GetRoute() => $"{RouteTemplate}?{nameof(CorrelationId)}={CorrelationId}";
  }
}
