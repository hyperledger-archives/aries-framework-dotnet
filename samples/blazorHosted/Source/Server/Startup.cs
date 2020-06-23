namespace BlazorHosted.Server
{
  using AutoMapper;
  using BlazorHosted.Configuration;
  using BlazorHosted.Features.Bases;
  using BlazorHosted.Infrastructure;
  using FluentValidation.AspNetCore;
  using Hyperledger.Aries.Storage;
  using Jdenticon.AspNetCore;
  using MediatR;
  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.ResponseCompression;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Hosting;
  using Microsoft.Extensions.Options;
  using Microsoft.OpenApi.Models;
  using Swashbuckle.AspNetCore.Swagger;
  using System;
  using System.IO;
  using System.Linq;
  using System.Net.Mime;
  using System.Reflection;

  [System.Runtime.InteropServices.Guid("858D6DFD-C176-4A54-9EAD-CB7E80B98AFA")]
  public class Startup
  {
    private readonly IConfiguration Configuration;
    private const string SwaggerVersion = "v1";
    private string SwaggerApiTitle => $"BlazorHosted API {SwaggerVersion}";
    private string SwaggerEndPoint => $"/swagger/{SwaggerVersion}/swagger.json";

    public Startup(IConfiguration aConfiguration)
    {
      Configuration = aConfiguration;
    }

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
      ConfigureServicesSettings(aServiceCollection);
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

      aServiceCollection.AddLogging();

      aServiceCollection.Configure<ApiBehaviorOptions>
      (
        aApiBehaviorOptions => aApiBehaviorOptions.SuppressInferBindingSourcesForParameters = true
      );

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
      ConfigureAries(aServiceCollection);
    }

    private void ConfigureAries(IServiceCollection aServiceCollection)
    {
      IServiceProvider serviceProvider = aServiceCollection.BuildServiceProvider();
      AgentSettings agentSettings = serviceProvider.GetService<IOptions<AgentSettings>>().Value;

      aServiceCollection.AddAriesFramework
      (
        aAriesFrameworkBuilder =>
          aAriesFrameworkBuilder.RegisterAgent<AriesWebAgent>
          (
            aAgentOptions =>
            {
              aAgentOptions.AgentName = agentSettings.AgentName;
              aAgentOptions.IssuerKeySeed = agentSettings.IssuerKeySeed;
              //aAgentOptions.AgentName = "Alice"; // Get from Config based on ENV
              aAgentOptions.GenesisFilename = Path.GetFullPath(agentSettings.GenesisFilename);
              aAgentOptions.WalletConfiguration = new WalletConfiguration { Id = agentSettings.WalletId };
              // The following is set one time when first provisioned.
              // To reset it delete everything in the ~/.indy_client directory (this will delete all wallet records)
              aAgentOptions.EndpointUri = agentSettings.EndpointUri;
              aAgentOptions.PoolName = agentSettings.PoolName;
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

    private void ConfigureServicesSettings(IServiceCollection aServiceCollection)
    {
      aServiceCollection.AddOptions();
      //aServiceCollection.Configure<RootSettings>(Configuration);
      aServiceCollection.Configure<AgentSettings>(Configuration.GetSection(nameof(AgentSettings)));
    }
  }
}
