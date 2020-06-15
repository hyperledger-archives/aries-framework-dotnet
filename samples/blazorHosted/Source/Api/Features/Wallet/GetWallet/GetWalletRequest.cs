namespace BlazorHosted.Features.Wallets
{
  using MediatR;
  using BlazorHosted.Features.Bases;

  public class GetWalletRequest : BaseApiRequest, IRequest<GetWalletResponse>
  {
    public const string Route = "api/Wallets/GetWallet";

    /// <summary>
    /// The Number of days of forecasts to get
    /// </summary>
    /// <example>5</example>
    public int Days { get; set; }

    internal override string RouteFactory => $"{Route}?{nameof(Days)}={Days}&{nameof(Id)}={Id}";
  }
}