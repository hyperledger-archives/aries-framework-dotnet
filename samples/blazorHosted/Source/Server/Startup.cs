namespace BlazorHosted.Server
{
  using FluentValidation.AspNetCore;
  using MediatR;
  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.ResponseCompression;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Hosting;
  using Microsoft.OpenApi.Models;
  using Swashbuckle.AspNetCore.Swagger;
  using System;
  using System.IO;
  using System.Linq;
  using System.Net.Mime;
  using System.Reflection;
  using BlazorHosted.Features.Bases;
  using AutoMapper;
  using BlazorHosted.Infrastructure;
  using Hyperledger.Aries.Configuration;
  using Jdenticon.AspNetCore;

  public class Startup
  {
    const string SwaggerVersion = "v1";
    string SwaggerApiTitle => $"BlazorHosted API {SwaggerVersion}";
    string SwaggerEndPoint => $"/swagger/{SwaggerVersion}/swagger.json";

    public void Configure
    (
      IApplicationBuilder aApplicationBuilder,
      IWebHostEnvironment aWebHostEnvironment
    )
    {
      aApplicationBuilder.UseJdenticon();
      aApplicationBuilder.UseAriesFramework();
      // Enable middleware to serve generated Swagger as a JSON endpoint.
      aApplicationBuilder.UseSwagger();

      // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
      // specifying the Swagger JSON endpoint.
      aApplicationBuilder.UseSwaggerUI
      (
        aSwaggerUIOptions => aSwaggerUIOptions.SwaggerEndpoint(SwaggerEndPoint, SwaggerApiTitle)
      );

      aApplicationBuilder.UseResponseCompression();

      if (aWebHostEnvironment.IsDevelopment())
      {
        aApplicationBuilder.UseDeveloperExceptionPage();
        aApplicationBuilder.UseWebAssemblyDebugging();
      }

      aApplicationBuilder.UseRouting();
      aApplicationBuilder.UseEndpoints
      (
        aEndpointRouteBuilder =>
        {
          aEndpointRouteBuilder.MapControllers();
          aEndpointRouteBuilder.MapBlazorHub();
          aEndpointRouteBuilder.MapFallbackToPage("/_Host");
        }
      );
      aApplicationBuilder.UseStaticFiles();
      aApplicationBuilder.UseBlazorFrameworkFiles();
    }

    public void ConfigureServices(IServiceCollection aServiceCollection)
    {
      ConfigureAries(aServiceCollection);
      aServiceCollection.AddRazorPages();
      aServiceCollection.AddServerSideBlazor();
      aServiceCollection.AddMvc()
        .AddFluentValidation
        (
          aFluentValidationMvcConfiguration =>
          {
            aFluentValidationMvcConfiguration.RegisterValidatorsFromAssemblyContaining<Startup>();
            aFluentValidationMvcConfiguration.RegisterValidatorsFromAssemblyContaining<BaseRequest>();
          }
        );

      aServiceCollection.Configure<ApiBehaviorOptions>
      (
        aApiBehaviorOptions => aApiBehaviorOptions.SuppressInferBindingSourcesForParameters = true);

      aServiceCollection.AddResponseCompression
      (
        aResponseCompressionOptions =>
          aResponseCompressionOptions.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat
          (
            new[] { MediaTypeNames.Application.Octet }
          )
      );

      Client.Program.ConfigureServices(aServiceCollection);

      aServiceCollection.AddMediatR(typeof(Startup).Assembly);

      aServiceCollection.AddAutoMapper(typeof(MappingProfile).Assembly);

      aServiceCollection.Scan
      (
        aTypeSourceSelector => aTypeSourceSelector
          .FromAssemblyOf<Startup>()
          .AddClasses()
          .AsSelf()
          .WithScopedLifetime()
      );
      ConfigureSwagger(aServiceCollection);
    }

    private void ConfigureAries(IServiceCollection aServiceCollection)
    {
      aServiceCollection.AddAriesFramework
      (
        aAriesFrameworkBuilder =>
          aAriesFrameworkBuilder.RegisterAgent
          (
            aAgentOptions =>
            {
              aAgentOptions.GenesisFilename = Path.GetFullPath("pool_genesis.txn");
              //TODO update this to use the current Kestrel setting which are not available in ConfigureServices
              // Or use the same Appsetting that Kestrel does to determine the port
              aAgentOptions.EndpointUri = "https://localhost:5001/"; // Is MyKestrel Enpoint. 
            }
          )
      );
    }

    private void ConfigureSwagger(IServiceCollection aServiceCollection)
    {
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

          // Set the comments path for the Swagger JSON and UI.
          string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
          string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
          aSwaggerGenOptions.IncludeXmlComments(xmlPath);

          // Set the comments path for the Swagger JSON and UI from API.
          xmlFile = $"{typeof(BaseRequest).Assembly.GetName().Name}.xml";
          xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
          aSwaggerGenOptions.IncludeXmlComments(xmlPath);

          aSwaggerGenOptions.AddFluentValidationRules();

          aSwaggerGenOptions
            .OrderActionsBy
            (
              aApiDescription => 
                $"{aApiDescription.GroupName}{aApiDescription.HttpMethod}{aApiDescription.RelativePath.Contains("{")}{aApiDescription.RelativePath}"
            );

        }
      );
    }
  }
}
