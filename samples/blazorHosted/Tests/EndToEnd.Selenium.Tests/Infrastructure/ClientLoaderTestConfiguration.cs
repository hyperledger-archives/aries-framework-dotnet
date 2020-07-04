namespace Hyperledger.Aries.OpenApi.EndToEnd.Tests.Infrastructure
{
  using System;
  using Hyperledger.Aries.OpenApi.Features.ClientLoaders;

  public class TestClientLoaderConfiguration : IClientLoaderConfiguration
  {
    public TimeSpan DelayTimeSpan => TimeSpan.FromMilliseconds(10);
  }
}
