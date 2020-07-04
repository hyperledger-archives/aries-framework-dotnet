namespace Microsoft.Extensions.DependencyInjection
{
  using FluentValidation;
  using Hyperledger.Aries.OpenApi.Configuration;
  using Hyperledger.Aries.OpenApi.Features.Bases;
  using MediatR;
  using Microsoft.OpenApi.Models;
  using Swashbuckle.AspNetCore.Swagger;
  using System;
  using System.IO;
  using System.Reflection;

  /// <summary>
  /// ServiceCollection Extension Methods
  /// </summary>
  public static class ServiceCollectionExtensions
  {
    private const string SwaggerVersion = "v1";

    /// <summary>
    /// Register Aries Open Api services
    /// </summary>
    /// <param name="aServiceCollection"></param>
    /// <param name="aMvcBuilder"></param>
    /// <param name="aConfigureAriesOpenApiOptionsAction"></param>
    public static IServiceCollection AddAriesOpenApi
    (
      this IServiceCollection aServiceCollection,
      IMvcBuilder aMvcBuilder,
      Action<AriesOpenApiOptions> aConfigureAriesOpenApiOptionsAction = null
    )
    {
      var ariesOpenApiOptions = new AriesOpenApiOptions();
      aConfigureAriesOpenApiOptionsAction?.Invoke(ariesOpenApiOptions);

      aMvcBuilder
        .AddApplicationPart(typeof(AriesOpenApiOptions).Assembly);

      aServiceCollection.AddValidatorsFromAssemblyContaining<BaseRequest>();
      aServiceCollection.AddValidatorsFromAssemblyContaining<AriesOpenApiOptions>();

      aServiceCollection.AddMediatR(typeof(BaseError).Assembly);

      ConfigureAriesOpenApiOptions(aServiceCollection, ariesOpenApiOptions);

      if (ariesOpenApiOptions.UseSwaggerUi)
      {
        ConfigureSwagger(aServiceCollection, ariesOpenApiOptions);
      }

      return aServiceCollection;
    }

    private static string ConfigureAriesOpenApiOptions(IServiceCollection aServiceCollection, AriesOpenApiOptions aAriesOpenApiOptions)
    {
      aAriesOpenApiOptions.SwaggerApiTitle = $"Aries Open API {SwaggerVersion}";
      aAriesOpenApiOptions.RoutePrefix = $"{BaseRequest.BaseUri}swagger";
      aAriesOpenApiOptions.SwaggerEndPoint = $"/{aAriesOpenApiOptions.RoutePrefix}/{SwaggerVersion}/swagger.json";
      aServiceCollection.AddSingleton<AriesOpenApiOptions>(aAriesOpenApiOptions);
      return SwaggerVersion;
    }

    private static void ConfigureSwagger
        (
      IServiceCollection aServiceCollection,
      AriesOpenApiOptions aAriesOpenApiOptions
    )
    {
      aServiceCollection.AddSwaggerGen
        (
          aSwaggerGenOptions =>
          {
            aSwaggerGenOptions
              .SwaggerDoc
              (
                SwaggerVersion,
                new OpenApiInfo { Title = aAriesOpenApiOptions.SwaggerApiTitle, Version = SwaggerVersion }
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
