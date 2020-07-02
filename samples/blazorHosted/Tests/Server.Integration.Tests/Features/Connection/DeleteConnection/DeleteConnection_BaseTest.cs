namespace BlazorHosted.Server.Integration.Tests.Infrastructure
{
  using BlazorHosted.Features.Connections;
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
