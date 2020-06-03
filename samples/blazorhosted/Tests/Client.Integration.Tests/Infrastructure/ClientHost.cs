namespace blazorhosted.Client.Integration.Tests.Infrastructure
{
  using Microsoft.Extensions.DependencyInjection;
  using System;

  [NotTest]
  public class ClientHost
  {

    public ClientHost(ServiceProvider aServiceProvider)
    {
      ServiceProvider = aServiceProvider;
    }

    public IServiceProvider ServiceProvider { get; }
  }
}
