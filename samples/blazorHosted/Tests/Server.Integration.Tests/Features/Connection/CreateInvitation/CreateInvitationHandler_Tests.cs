namespace CreateInvitationHandler
{
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc.Testing;
  using BlazorHosted.Server.Integration.Tests.Infrastructure;
  using BlazorHosted.Features.Connections;
  using BlazorHosted.Server;
  using FluentAssertions;
  using Newtonsoft.Json;
  using Hyperledger.Aries.Features.DidExchange;

  public class Handle_Returns : BaseTest
  {
    private readonly CreateInvitationRequest CreateInvitationRequest;

    public Handle_Returns
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerSettings aJsonSerializerSettings
    ) : base(aWebApplicationFactory, aJsonSerializerSettings)
    {
      // Set Valid values here
      var inviteConfiguration = new InviteConfiguration
      {
        AutoAcceptConnection = true,
      };

      inviteConfiguration.MyAlias.Name = "Faber";
      CreateInvitationRequest = new CreateInvitationRequest(inviteConfiguration);
    }

    public async Task CreateInvitationResponse()
    {
      CreateInvitationResponse createInvitationResponse = await Send(CreateInvitationRequest);

      ValidateCreateInvitationResponse(createInvitationResponse);
    }

    private void ValidateCreateInvitationResponse(CreateInvitationResponse aCreateInvitationResponse)
    {
      aCreateInvitationResponse.CorrelationId.Should().Be(CreateInvitationRequest.CorrelationId);
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