namespace Hyperledger.Aries.OpenApi.Features.CredentialDefinitions
{
  using Hyperledger.Aries.OpenApi.Features.Bases;
  using MediatR;

  public class GetCredentialDefinitionsRequest : BaseApiRequest, IRequest<GetCredentialDefinitionsResponse>
  {
    public const string RouteTemplate = BaseRequest.BaseUri + "credential-definitions";

    internal override string GetRoute() => $"{RouteTemplate}?{nameof(CorrelationId)}={CorrelationId}";
  }
}
