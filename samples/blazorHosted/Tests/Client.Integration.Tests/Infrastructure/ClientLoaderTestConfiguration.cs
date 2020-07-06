namespace Hyperledger.Aries.AspNetCore.Client.Integration.Tests.Infrastructure
{
  using System;
  using Hyperledger.Aries.AspNetCore.Features.ClientLoaders;

  [NotTest]
  public class ClientLoaderTestConfiguration : IClientLoaderConfiguration
  {
    public TimeSpan DelayTimeSpan => TimeSpan.FromMilliseconds(10);
  }
}
