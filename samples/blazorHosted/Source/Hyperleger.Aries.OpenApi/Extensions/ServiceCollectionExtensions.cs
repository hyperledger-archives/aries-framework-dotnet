namespace Microsoft.Extensions.DependencyInjection
{
  using BlazorHosted.Features.Bases;
  using Hyperledger.Aries.Configuration;
  using MediatR;
  using Microsoft.OpenApi.Models;
  using Swashbuckle.AspNetCore.Swagger;
  using System;
  using System.IO;
  using System.Reflection;

  public static class ServiceCollectionExtensions
  {
    /// <summary>
    /// Register Aries Open Api services
    /// </summary>
    /// <param name="aServiceCollection"></param>
    /// <param name="aConfigureAriesOpenApiOptionsAction"></param>
    public static IServiceCollection AddAriesOpenApi
    (
      this IServiceCollection aServiceCollection,
      Action<AriesOpenApiOptions> aConfigureAriesOpenApiOptionsAction = null
    )
    {
      var ariesOpenApiOptions = new AriesOpenApiOptions();
      aConfigureAriesOpenApiOptionsAction?.Invoke(ariesOpenApiOptions);

      aServiceCollection.AddControllers().AddApplicationPart(typeof(BaseEndpoint<,>).Assembly);
      aServiceCollection.AddMediatR(typeof(BaseEndpoint<,>).Assembly);

      ConfigureSwagger(aServiceCollection, ariesOpenApiOptions);

      return aServiceCollection;
    }

    private const string SwaggerVersion = "v1";

    private static void ConfigureSwagger
    (
      IServiceCollection aServiceCollection, 
      AriesOpenApiOptions aAriesOpenApiOptions
    )
    {

      string SwaggerApiTitle = $"BlazorHosted API {SwaggerVersion}";
      string SwaggerEndPoint = $"/swagger/{SwaggerVersion}/swagger.json";

      // Register the Swagger generator, defining 1 or more Swagger documents
      aServiceCollection.AddSwaggerGen
        (
          aSwaggerGenOptions =>
          {
            aSwaggerGenOptions
              .SwaggerDoc
              (
                SwaggerVersion,
                new OpenApiInfo { Title = SwaggerApiTitle, Version = SwaggerVersion }
              );

            aSwaggerGenOptions.EnableAnnotations();

            string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            aSwaggerGenOptions.IncludeXmlComments(xmlPath);

            xmlFile = $"{typeof(BaseRequest).Assembly.GetName().Name}.xml";
            xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            aSwaggerGenOptions.IncludeXmlComments(xmlPath);

            xmlFile = $"{typeof(BaseEndpoint<,>).Assembly.GetName().Name}.xml";
            xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            aSwaggerGenOptions.IncludeXmlComments(xmlPath);

            aSwaggerGenOptions.AddFluentValidationRules();

            aSwaggerGenOptions
              .OrderActionsBy
              (
                aApiDescription =>
                  $"{aApiDescription.GroupName}{aApiDescription.HttpMethod}" +
                  $"{aApiDescription.RelativePath.Contains("{")}{aApiDescription.RelativePath}"
              );
          }
        );
    }

  }
}