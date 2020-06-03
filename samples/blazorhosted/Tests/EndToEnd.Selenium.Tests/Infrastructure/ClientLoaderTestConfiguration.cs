namespace blazorhosted.EndToEnd.Tests.Infrastructure
{
  using System;
  using blazorhosted.Features.ClientLoaders;

  public class TestClientLoaderConfiguration : IClientLoaderConfiguration
  {
    public TimeSpan DelayTimeSpan => TimeSpan.FromMilliseconds(10);
  }
}
