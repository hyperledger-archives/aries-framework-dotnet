namespace BlazorHosted.Server.Integration.Tests.Infrastructure
{
  using BlazorHosted.Features.Connections;
  using FluentAssertions;
  using Hyperledger.Aries.Features.DidExchange;
  using System.Threading.Tasks;

  public partial class BaseTest
  {
    internal async Task CreateAnInvitation()
    {
      _ = await Send(CreateValidCreateInvitationRequest());
    }

    internal CreateInvitationRequest CreateValidCreateInvitationRequest()
    {
      var inviteConfiguration = new InviteConfiguration
      {
        AutoAcceptConnection = true,
      };

      inviteConfiguration.MyAlias.Name = "Faber";
      return new CreateInvitationRequest(inviteConfiguration);
    }

    internal void ValidateCreateInvitationResponse(CreateInvitationRequest aCreateInvitationRequest, CreateInvitationResponse aCreateInvitationResponse)
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
