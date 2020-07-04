namespace Hyperledger.Aries.OpenApi.Client.Integration.Tests.Infrastructure
{
  using System;
  using Hyperledger.Aries.OpenApi.Features.ClientLoaders;

  [NotTest]
  public class ClientLoaderTestConfiguration : IClientLoaderConfiguration
  {
    public TimeSpan DelayTimeSpan => TimeSpan.FromMilliseconds(10);
  }
}
