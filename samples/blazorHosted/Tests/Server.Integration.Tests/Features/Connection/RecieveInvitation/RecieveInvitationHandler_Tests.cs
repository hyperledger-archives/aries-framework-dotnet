//namespace RecieveInvitationHandler
//{
//  using System.Threading.Tasks;
//  using System.Text.Json;
//  using Microsoft.AspNetCore.Mvc.Testing;
//  using Hyperledger.Aries.OpenApi.Server.Integration.Tests.Infrastructure;
//  using Hyperledger.Aries.OpenApi.Features.Connections;
//  using Hyperledger.Aries.OpenApi.Server;
//  using FluentAssertions;

//  public class Handle_Returns : BaseTest
//  {
//    private readonly RecieveInvitationRequest RecieveInvitationRequest;

//    public Handle_Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      RecieveInvitationRequest = new RecieveInvitationRequest { Days = 10 };
//    }

//    public async Task RecieveInvitationResponse()
//    {
//      RecieveInvitationResponse RecieveInvitationResponse = await Send(RecieveInvitationRequest);

//      ValidateRecieveInvitationResponse(RecieveInvitationResponse);
//    }

//    private void ValidateRecieveInvitationResponse(RecieveInvitationResponse aRecieveInvitationResponse)
//    {
//      aRecieveInvitationResponse.CorrelationId.Should().Be(RecieveInvitationRequest.CorrelationId);
//      // check Other properties here
//    }

//  }
//}