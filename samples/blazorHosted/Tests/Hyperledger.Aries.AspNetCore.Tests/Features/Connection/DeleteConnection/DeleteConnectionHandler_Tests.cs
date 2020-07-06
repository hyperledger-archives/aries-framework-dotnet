namespace DeleteConnectionHandler
{
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc.Testing;
  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
  using Hyperledger.Aries.AspNetCore.Features.Connections;
  using Hyperledger.Aries.AspNetCore.Server;
  using FluentAssertions;
  using Newtonsoft.Json;

  public class Handle_Returns : BaseTest
  {
    private DeleteConnectionRequest DeleteConnectionRequest { get; set; }
    private CreateInvitationResponse CreateInvitationResponse { get; set; }

    public Handle_Returns
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerSettings aJsonSerializerSettings
    ) : base(aWebApplicationFactory, aJsonSerializerSettings)
    {
      DeleteConnectionRequest = new DeleteConnectionRequest("ConnectionId");
    }

    public async Task DeleteConnectionResponse()
    {
      DeleteConnectionRequest.ConnectionId = CreateInvitationResponse.ConnectionRecord.Id;

      DeleteConnectionResponse deleteConnectionResponse = await Send(DeleteConnectionRequest);

      ValidateDeleteConnectionResponse(DeleteConnectionRequest, deleteConnectionResponse);

      //Confirm it is deleted

      GetConnectionResponse getConnectionResponse = await Send(new GetConnectionRequest(CreateInvitationResponse.ConnectionRecord.Id));
      getConnectionResponse.ConnectionRecord.Should().BeNull();

    }


    public async Task Setup()
    {
      await ResetAgent();
      CreateInvitationResponse = await CreateAnInvitation();
    }
  }
}