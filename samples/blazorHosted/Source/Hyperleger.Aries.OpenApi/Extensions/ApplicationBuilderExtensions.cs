namespace Microsoft.Extensions.DependencyInjection
{
  using Hyperledger.Aries.OpenApi.Configuration;
  using Microsoft.AspNetCore.Builder;

  /// <summary>
  /// <see cref="IServiceCollection"/> extension methods
  /// </summary>
  public static class ApplicationBuilderExtensions
  {
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
      aApplicationBuilder.UseRouting();
      aApplicationBuilder.UseEndpoints(aEndpointRouteBuilder => aEndpointRouteBuilder.MapControllers());
    }
  }
}
