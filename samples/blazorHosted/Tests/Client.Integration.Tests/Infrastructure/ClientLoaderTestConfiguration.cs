namespace BlazorHosted.Client.Integration.Tests.Infrastructure
{
  using System;
  using BlazorHosted.Features.ClientLoaders;

  [NotTest]
  public class ClientLoaderTestConfiguration : IClientLoaderConfiguration
  {
    public TimeSpan DelayTimeSpan => TimeSpan.FromMilliseconds(10);
  }
}
