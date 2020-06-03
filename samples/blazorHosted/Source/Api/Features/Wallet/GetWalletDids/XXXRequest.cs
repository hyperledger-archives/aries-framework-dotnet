namespace BlazorHosted.Features.Wallets
{
  using MediatR;
  using System.Text.Json.Serialization;
  using BlazorHosted.Features.Bases;

  public class GetWalletDidsRequest : BaseRequest, IRequest<GetWalletDidsResponse>
  {
    public const string Route = "xxx/YYY";

    [JsonIgnore]
    public string RouteFactory => $"{Route}?{nameof(Id)}={Id}";
  }
}
