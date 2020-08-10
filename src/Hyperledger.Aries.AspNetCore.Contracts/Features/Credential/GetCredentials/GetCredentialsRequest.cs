namespace Hyperledger.Aries.AspNetCore.Features.Credentials
{
  using MediatR;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public class GetCredentialsRequest : BaseApiRequest, IRequest<GetCredentialsResponse>
  {
    public const string RouteTemplate = BaseRequest.BaseUri + "credentials";

    internal override string GetRoute() => $"{RouteTemplate}?{nameof(CorrelationId)}={CorrelationId}";
  }
}