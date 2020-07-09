//namespace AcceptInvitationEndpoint
//{
//  using FluentAssertions;
//  using Microsoft.AspNetCore.Mvc.Testing;
//  using System.Net;
//  using System.Net.Http;
//  using System.Text.Json;
//  using System.Threading.Tasks;
//  using Hyperledger.Aries.AspNetCore.Features.Connections;
//  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
//  using Hyperledger.Aries.AspNetCore.Server;

//  public class Returns : BaseTest
//  {
//    private readonly AcceptInvitationRequest AcceptInvitationRequest;

//    public Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      AcceptInvitationRequest = new AcceptInvitationRequest { Days = 10 };
//    }

//    public async Task AcceptInvitationResponse()
//    {
//      AcceptInvitationResponse AcceptInvitationResponse =
//        await GetJsonAsync<AcceptInvitationResponse>(AcceptInvitationRequest.GetRoute());

//      ValidateAcceptInvitationResponse(AcceptInvitationResponse);
//    }

//    public async Task ValidationError()
//    {
//      // Set invalid value
//      AcceptInvitationRequest.Days = -1;

//      HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(AcceptInvitationRequest.GetRoute());

//      string json = await httpResponseMessage.Content.ReadAsStringAsync();

//      httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//      json.Should().Contain("errors");
//      json.Should().Contain(nameof(AcceptInvitationRequest.Days));
//    }

//    private void ValidateAcceptInvitationResponse(AcceptInvitationResponse aAcceptInvitationResponse)
//    {
//      aAcceptInvitationResponse.CorrelationId.Should().Be(AcceptInvitationRequest.CorrelationId);
//      // check Other properties here
//    }
//  }
//}