namespace BlazorHosted.Features.XXXs
{
  using MediatR;
  using System.Text.Json.Serialization;
  using BlazorHosted.Features.Bases;

  public class XXXRequest : BaseRequest, IRequest<XXXResponse>
  {
    public const string Route = "xxx/YYY";

    [JsonIgnore]
    public string RouteFactory => $"{Route}?{nameof(Id)}={Id}";
  }
}
