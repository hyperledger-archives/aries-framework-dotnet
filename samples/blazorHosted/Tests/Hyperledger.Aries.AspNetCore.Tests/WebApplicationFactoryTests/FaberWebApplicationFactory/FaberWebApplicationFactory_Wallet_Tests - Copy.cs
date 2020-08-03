namespace FaberWebApplicationFactory_
{
  using Hyperledger.Aries.AspNetCore.Configuration;
  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Options;
  using Newtonsoft.Json;

  public class Wallet_Should : BaseTest
  {
    private readonly AgentSettings AgentSettings;

    public Wallet_Should
    (
      FaberWebApplicationFactory aFaberWebApplicationFactory,
      JsonSerializerSettings aJsonSerializerSettings
    ) : base(aFaberWebApplicationFactory, aJsonSerializerSettings)
    {
      AgentSettings = aFaberWebApplicationFactory.Services.GetService<IOptions<AgentSettings>>().Value;
    }
  }
}
