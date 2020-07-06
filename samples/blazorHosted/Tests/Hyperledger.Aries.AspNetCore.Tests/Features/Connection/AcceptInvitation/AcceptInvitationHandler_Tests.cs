//namespace AcceptInvitationHandler
//{
//  using System.Threading.Tasks;
//  using System.Text.Json;
//  using Microsoft.AspNetCore.Mvc.Testing;
//  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
//  using Hyperledger.Aries.AspNetCore.Features.Connections;
//  using Hyperledger.Aries.AspNetCore.Server;
//  using FluentAssertions;

//  public class Handle_Returns : BaseTest
//  {
//    private readonly AcceptInvitationRequest AcceptInvitationRequest;

//    public Handle_Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      AcceptInvitationRequest = new AcceptInvitationRequest { Days = 10 };
//    }

//    public async Task AcceptInvitationResponse()
//    {
//      AcceptInvitationResponse AcceptInvitationResponse = await Send(AcceptInvitationRequest);

//      ValidateAcceptInvitationResponse(AcceptInvitationResponse);
//    }

//    private void ValidateAcceptInvitationResponse(AcceptInvitationResponse aAcceptInvitationResponse)
//    {
//      aAcceptInvitationResponse.CorrelationId.Should().Be(AcceptInvitationRequest.CorrelationId);
//      // check Other properties here
//    }

//  }
//}