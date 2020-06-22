namespace BlazorHosted.Pages
{
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;
  using static BlazorHosted.Features.WeatherForecasts.WeatherForecastsState;

  public partial class WeatherForecastsPage : BaseComponent
  {
    public const string RouteTemplate = "/weatherforecasts";
    public static string GetRoute() => RouteTemplate;

    protected override async Task OnInitializedAsync() =>
      await Mediator.Send(new FetchWeatherForecastsAction());
  }
}
