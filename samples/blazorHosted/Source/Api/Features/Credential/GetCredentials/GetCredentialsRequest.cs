namespace BlazorHosted.Features.Credentials
{
  using MediatR;
  using BlazorHosted.Features.Bases;

  public class GetCredentialsRequest : BaseApiRequest, IRequest<GetCredentialsResponse>
  {
    public const string Route = "api/credentials";

    internal override string RouteFactory => $"{Route}?{nameof(CorrelationId)}={CorrelationId}";
  }
}