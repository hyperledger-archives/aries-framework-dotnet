namespace Microsoft.Extensions.DependencyInjection
{
  using Hyperledger.Aries.AspNetCore;
  using Hyperledger.Aries.Configuration;
  using Hyperledger.Aries.AspNetCore.Configuration;
  using Microsoft.AspNetCore.Builder;
  using Microsoft.Extensions.Options;

  /// <summary>
  /// <see cref="IServiceCollection"/> extension methods
  /// </summary>
  public static class ApplicationBuilderExtensions
  {
    /// <summary>
    /// Allows default agent configuration
    /// </summary>
    /// <param name="aApplicationBuilder">App.</param>
    public static void UseAriesFramework(this IApplicationBuilder aApplicationBuilder) => 
      UseAriesFramework<AgentMiddleware>(aApplicationBuilder);

    /// <summary>
    /// Allows agent configuration by specifying a custom middleware
    /// </summary>
    /// <param name="aApplicationBuilder">App.</param>
    public static void UseAriesFramework<T>(this IApplicationBuilder aApplicationBuilder)
    {
      AgentOptions options = 
        aApplicationBuilder.ApplicationServices.GetRequiredService<IOptions<AgentOptions>>().Value;

      aApplicationBuilder.UseMiddleware<T>();
      aApplicationBuilder.MapWhen
      (
          aHttpContext => aHttpContext.Request.Path.ToUriComponent().Contains(options.RevocationRegistryUriPath),
          aApplicationBuilder_ => aApplicationBuilder_.UseMiddleware<TailsMiddleware>()
      );
    }

    /// <summary>
    /// Allows agent configuration by specifying a custom middleware
    /// </summary>
    /// <remarks>Requires corresponding call to AddAriesOpenApi in ConfigureServices</remarks>
    /// <param name="aApplicationBuilder">App.</param>
    public static void UseAriesOpenApi(this IApplicationBuilder aApplicationBuilder)
    {
      AriesOpenApiOptions ariesOpenApiOptions =
        aApplicationBuilder.ApplicationServices.GetRequiredService<AriesOpenApiOptions>();

      if (ariesOpenApiOptions.UseSwaggerUi)
      {
        aApplicationBuilder
        .UseSwagger
        (
          aSwaggerOptions =>
            aSwaggerOptions.RouteTemplate = $"{ariesOpenApiOptions.RoutePrefix}/{{documentname}}/swagger.json"
        );

        // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
        // specifying the Swagger JSON endpoint.
        aApplicationBuilder.UseSwaggerUI
        (
          aSwaggerUIOptions =>
          {
            aSwaggerUIOptions.SwaggerEndpoint(ariesOpenApiOptions.SwaggerEndPoint, ariesOpenApiOptions.SwaggerApiTitle);
            aSwaggerUIOptions.RoutePrefix = ariesOpenApiOptions.RoutePrefix;
          }
        );
      }
      //aApplicationBuilder.UseRouting();
      //aApplicationBuilder.UseEndpoints(aEndpointRouteBuilder => aEndpointRouteBuilder.MapControllers());
    }
  }
}
