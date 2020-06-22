namespace BlazorHosted.Client
{
  using BlazorHosted.Components;
  using BlazorHosted.Features.ClientLoaders;
  using BlazorHosted.Features.EventStreams;
  using BlazorState;
  using MediatR;
  using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
  using Microsoft.Extensions.DependencyInjection;
  using PeterLeslieMorris.Blazor.Validation;
  using System;
  using System.Net.Http;
  using System.Reflection;
  using System.Threading.Tasks;

  public class Program
  {
    public static void ConfigureServices(IServiceCollection aServiceCollection)
    {
      aServiceCollection.AddBlazorState
      (
        (aOptions) =>
        {
#if ReduxDevToolsEnabled
          aOptions.UseReduxDevToolsBehavior = true;
#endif
          aOptions.Assemblies =
            new Assembly[]
            {
                typeof(Program).GetTypeInfo().Assembly,
            };
        }
      );

      aServiceCollection.AddFormValidation
      (
        aValidationConfiguration =>
          aValidationConfiguration
          .AddFluentValidation
          (
            typeof(Api.AssemblyAnnotations).Assembly,
            typeof(Client.AssemblyAnnotations).Assembly
          )
      );

      aServiceCollection.AddScoped(typeof(IPipelineBehavior<,>), typeof(EventStreamBehavior<,>));
      aServiceCollection.AddScoped<ClientLoader>();
      aServiceCollection.AddScoped<IClientLoaderConfiguration, ClientLoaderConfiguration>();
    }

    public static async Task Main(string[] aArgumentArray)
    {
      var builder = WebAssemblyHostBuilder.CreateDefault(aArgumentArray);
      builder.RootComponents.Add<App>("app");
      builder.Services.AddSingleton
      (
        new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) }
      );
      ConfigureServices(builder.Services);

      WebAssemblyHost host = builder.Build();
      await host.RunAsync();
    }
  }
}
