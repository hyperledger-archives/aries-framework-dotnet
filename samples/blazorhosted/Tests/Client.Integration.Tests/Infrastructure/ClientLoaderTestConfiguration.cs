namespace blazorhosted.Client.Integration.Tests.Infrastructure
{
  using System;
  using blazorhosted.Features.ClientLoaders;

  [NotTest]
  public class ClientLoaderTestConfiguration : IClientLoaderConfiguration
  {
    public TimeSpan DelayTimeSpan => TimeSpan.FromMilliseconds(10);
  }
}
