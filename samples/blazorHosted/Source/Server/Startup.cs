namespace Hyperledger.Aries.AspNetCore.Server
{
  using AutoMapper;
  using FluentValidation.AspNetCore;
  using Hyperledger.Aries.AspNetCore.Configuration;
  using Hyperledger.Aries.AspNetCore.Infrastructure;
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
  using System;
  using System.IO;
  using System.Linq;
  using System.Net.Mime;

  [System.Runtime.InteropServices.Guid("858D6DFD-C176-4A54-9EAD-CB7E80B98AFA")]
  public class Startup
  {
    private readonly IConfiguration Configuration;

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
      aApplicationBuilder.UseAriesOpenApi();
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

      IMvcBuilder mvcBuilder = aServiceCollection.AddMvc()
        .AddNewtonsoftJson()
        .AddFluentValidation
        (
          aFluentValidationMvcConfiguration =>
            aFluentValidationMvcConfiguration.RegisterValidatorsFromAssemblyContaining<Startup>()
        )
        .AddAriesOpenApi(a => a.UseSwaggerUi = true);

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
              aAgentOptions.GenesisFilename = Path.GetFullPath(agentSettings.GenesisFilename);
              aAgentOptions.WalletConfiguration = new WalletConfiguration { Id = agentSettings.WalletId };
              aAgentOptions.WalletCredentials = new WalletCredentials { Key = agentSettings.WalletKey };
              // The following is set one time when first provisioned.
              // To reset it delete everything in the ~/.indy_client directory (this will delete all wallet records)
              aAgentOptions.EndpointUri = agentSettings.EndpointUri;
              aAgentOptions.PoolName = agentSettings.PoolName;
            }
          )
      );
    }

    private void ConfigureServicesSettings(IServiceCollection aServiceCollection)
    {
      aServiceCollection.AddOptions();
      aServiceCollection.Configure<AgentSettings>(Configuration.GetSection(nameof(AgentSettings)));
    }
  }
}
