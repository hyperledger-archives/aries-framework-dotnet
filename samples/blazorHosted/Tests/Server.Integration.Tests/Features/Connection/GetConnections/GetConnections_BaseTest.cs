namespace Hyperledger.Aries.OpenApi.Server.Integration.Tests.Infrastructure
{
  using Hyperledger.Aries.OpenApi.Features.Connections;
  using FluentAssertions;

  public partial class BaseTest
  {
    internal static GetConnectionsRequest CreateValidGetConnectionsRequest()
    {
      return new GetConnectionsRequest();
    }

    internal static void ValidateGetConnectionsResponse(GetConnectionsRequest aGetConnectionsRequest, GetConnectionsResponse aGetConnectionsResponse)
    {
      aGetConnectionsResponse.CorrelationId.Should().Be(aGetConnectionsRequest.CorrelationId);
      aGetConnectionsResponse.ConnectionRecords.Should().NotBeNull();
      aGetConnectionsResponse.ConnectionRecords.Count.Should().Be(1);
    }
  }
}
