namespace blazorhosted.Pages
{
  using System.Threading.Tasks;
  using blazorhosted.Features.Bases;
  using static blazorhosted.Features.WeatherForecasts.WeatherForecastsState;

  public partial class WeatherForecastsPage : BaseComponent
  {
    public const string Route = "/weatherforecasts";

    protected override async Task OnInitializedAsync() =>
      await Mediator.Send(new FetchWeatherForecastsAction());
  }
}
