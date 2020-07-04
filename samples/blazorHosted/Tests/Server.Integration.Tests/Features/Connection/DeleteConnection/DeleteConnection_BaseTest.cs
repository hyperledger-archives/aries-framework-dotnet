namespace Hyperledger.Aries.OpenApi.Server.Integration.Tests.Infrastructure
{
  using Hyperledger.Aries.OpenApi.Features.Connections;
  using FluentAssertions;
  public partial class BaseTest
  {
    internal static void ValidateDeleteConnectionResponse
    (
      DeleteConnectionRequest aDeleteConnectionRequest, 
      DeleteConnectionResponse aDeleteConnectionResponse
    )
    {
      aDeleteConnectionResponse.CorrelationId.Should().Be(aDeleteConnectionRequest.CorrelationId);
    }
  }
}
