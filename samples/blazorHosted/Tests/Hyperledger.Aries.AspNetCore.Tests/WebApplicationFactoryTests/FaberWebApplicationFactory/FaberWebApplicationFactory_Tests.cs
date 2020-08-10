namespace FaberWebApplicationFactory_
{
  using Hyperledger.Aries.AspNetCore.Configuration;
  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
  using FluentAssertions;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Options;
  using Newtonsoft.Json;

  public class AgentSettings_Should : BaseTest
  {
    private readonly AgentSettings AgentSettings;

    public AgentSettings_Should
    (
      FaberWebApplicationFactory aFaberWebApplicationFactory,
      JsonSerializerSettings aJsonSerializerSettings
    ) : base(aFaberWebApplicationFactory, aJsonSerializerSettings)
    {
      AgentSettings = aFaberWebApplicationFactory.Services.GetService<IOptions<AgentSettings>>().Value;
    }

    public void Be_Valid()
    {
      AgentSettings.AgentName.Should().Be("Faber");
    }
  }
}
