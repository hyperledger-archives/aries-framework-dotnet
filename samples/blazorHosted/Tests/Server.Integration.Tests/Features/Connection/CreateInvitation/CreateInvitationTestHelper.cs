namespace TestHelpers
{
  using BlazorHosted.Features.Connections;
  using FluentAssertions;
  using Hyperledger.Aries.Features.DidExchange;

  internal static class CreateInvitationTestHelper
  {
    internal static CreateInvitationRequest CreateValidCreateInvitationRequest()
    {
      var inviteConfiguration = new InviteConfiguration
      {
        AutoAcceptConnection = true,
      };

      inviteConfiguration.MyAlias.Name = "Faber";
      return new CreateInvitationRequest(inviteConfiguration);
    }

    internal static void ValidateCreateInvitationResponse(CreateInvitationRequest aCreateInvitationRequest, CreateInvitationResponse aCreateInvitationResponse)
    {
      aCreateInvitationResponse.CorrelationId.Should().Be(aCreateInvitationRequest.CorrelationId);
      aCreateInvitationResponse.ConnectionInvitationMessage.Id.Should().NotBeNullOrEmpty();
      aCreateInvitationResponse.ConnectionInvitationMessage.ImageUrl.Should().BeNull();
      aCreateInvitationResponse.ConnectionInvitationMessage.Label.Should().Be("Faber");
      aCreateInvitationResponse.ConnectionInvitationMessage.RecipientKeys.Count.Should().Be(1);
      aCreateInvitationResponse.ConnectionInvitationMessage.RoutingKeys.Count.Should().Be(1);
      aCreateInvitationResponse.ConnectionInvitationMessage.ServiceEndpoint.Should().Be("https://localhost:5551");
      aCreateInvitationResponse.ConnectionInvitationMessage.Type.Should().Be("did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/connections/1.0/invitation");

      aCreateInvitationResponse.InvitationUrl.Should().NotBeNull();
    }
  }
}
