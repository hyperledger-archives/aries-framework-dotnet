namespace BlazorHosted.EndToEnd.Tests.Infrastructure
{
  using System;
  using BlazorHosted.Features.ClientLoaders;

  public class TestClientLoaderConfiguration : IClientLoaderConfiguration
  {
    public TimeSpan DelayTimeSpan => TimeSpan.FromMilliseconds(10);
  }
}
