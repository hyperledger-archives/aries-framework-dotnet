namespace Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure
{
  using Hyperledger.Aries.AspNetCore.Features.Connections;
  using FluentAssertions;
  using Hyperledger.Aries.Features.DidExchange;
  using System;

  public partial class BaseTest
  {
    internal static GetConnectionRequest CreateValidGetConnectionRequest()
    {
      // This will validate but the GUID won't be in the wallet.
      return new GetConnectionRequest(aConnectionId: Guid.NewGuid().ToString());
    }

    internal static void ValidateGetConnectionResponse(GetConnectionRequest aGetConnectionRequest, GetConnectionResponse aGetConnectionResponse)
    {
      aGetConnectionResponse.CorrelationId.Should().Be(aGetConnectionRequest.CorrelationId);
      aGetConnectionResponse.ConnectionRecord.Should().NotBeNull();
      aGetConnectionResponse.ConnectionRecord.Alias.Name.Should().BeNull();
      aGetConnectionResponse.ConnectionRecord.Alias.ImageUrl.Should().BeNull();
      aGetConnectionResponse.ConnectionRecord.CreatedAtUtc.HasValue.Should().BeTrue();
      aGetConnectionResponse.ConnectionRecord.Endpoint.Should().BeNull();
      aGetConnectionResponse.ConnectionRecord.MultiPartyInvitation.Should().BeFalse();
      aGetConnectionResponse.ConnectionRecord.Id.Should().Be(aGetConnectionRequest.ConnectionId);
      aGetConnectionResponse.ConnectionRecord.MyDid.Should().BeNull();
      aGetConnectionResponse.ConnectionRecord.MyVk.Should().BeNull();
      aGetConnectionResponse.ConnectionRecord.State.Should().Be(ConnectionState.Invited);
      aGetConnectionResponse.ConnectionRecord.TheirDid.Should().BeNull();
      aGetConnectionResponse.ConnectionRecord.TheirVk.Should().BeNull();
      aGetConnectionResponse.ConnectionRecord.TypeName.Should().Be("AF.ConnectionRecord");
      aGetConnectionResponse.ConnectionRecord.UpdatedAtUtc.HasValue.Should().BeFalse();
    }
  }
}
