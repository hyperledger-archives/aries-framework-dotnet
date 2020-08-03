namespace Hyperledger.Aries.AspNetCore.Features.ClientLoaders
{
  using System;

  public class ClientLoaderConfiguration : IClientLoaderConfiguration
  {
    public TimeSpan DelayTimeSpan => TimeSpan.FromSeconds(10);
  }
}
