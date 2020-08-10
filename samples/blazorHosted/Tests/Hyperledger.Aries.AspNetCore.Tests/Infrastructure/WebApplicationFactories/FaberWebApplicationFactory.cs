namespace Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure
{
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.Mvc.Testing;

  [NotTest]
  public class FaberWebApplicationFactory : WebApplicationFactory<Startup>
  {
    protected override void ConfigureWebHost(IWebHostBuilder aWebHostBuilder)
    {
      aWebHostBuilder.UseEnvironment("Development");
    }
  }
}
