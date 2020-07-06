namespace Microsoft.Extensions.DependencyInjection
{
  using FluentValidation;
  using FluentValidation.AspNetCore;
  using Hyperledger.Aries.AspNetCore.Configuration;
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using MediatR;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.OpenApi.Models;
  using Swashbuckle.AspNetCore.Swagger;
  using System;
  using System.IO;
  using System.Linq;
  using System.Reflection;

  /// <summary>
  /// IMvcBuilder Extension Methods
  /// </summary>
  public static class MvcBuilderExtensions
  {
    private const string SwaggerVersion = "v1";

    /// <summary>
    /// Register Aries Open Api services
    /// </summary>
    /// <param name="aMvcBuilder"></param>
    /// <param name="aConfigureAriesOpenApiOptionsAction"></param>
    public static IMvcBuilder AddAriesOpenApi
    (
      this IMvcBuilder aMvcBuilder,
      Action<AriesOpenApiOptions> aConfigureAriesOpenApiOptionsAction = null
    )
    {
      IServiceCollection serviceCollection = aMvcBuilder.Services;

      bool hasAlreadyBeenRun = serviceCollection.Any(aServiceDescriptor => aServiceDescriptor.ServiceType == typeof(AriesOpenApiOptions));
      if (hasAlreadyBeenRun) return aMvcBuilder;
      
      var ariesOpenApiOptions = new AriesOpenApiOptions();
      aConfigureAriesOpenApiOptionsAction?.Invoke(ariesOpenApiOptions);

      aMvcBuilder
        .AddApplicationPart(typeof(AriesOpenApiOptions).Assembly);


      ConfigureFluentValidationServices(serviceCollection);
      serviceCollection.Configure<ApiBehaviorOptions>
      (
        aApiBehaviorOptions => aApiBehaviorOptions.SuppressInferBindingSourcesForParameters = true
      );

      serviceCollection.AddMediatR(typeof(BaseError).Assembly);

      ConfigureAriesOpenApiOptions(serviceCollection, ariesOpenApiOptions);

      if (ariesOpenApiOptions.UseSwaggerUi)
      {
        ConfigureSwagger(serviceCollection, ariesOpenApiOptions);
      }

      return aMvcBuilder;
    }

    private static void ConfigureFluentValidationServices(IServiceCollection aServiceCollection)
    {
      if (!aServiceCollection.Any(aServiceDescriptor => aServiceDescriptor.ServiceType == typeof(IValidatorFactory)))
      {
        throw new InvalidOperationException
        (
          $"You must call {nameof(FluentValidationMvcExtensions.AddFluentValidation)} " +
          $"prior to {nameof(AddAriesOpenApi)}"
        );
      }

      aServiceCollection.AddValidatorsFromAssemblyContaining<BaseRequest>();
      aServiceCollection.AddValidatorsFromAssemblyContaining<AriesOpenApiOptions>();
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
