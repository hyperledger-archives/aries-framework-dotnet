//namespace ReceiveInvitationEndpoint
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
//    private readonly ReceiveInvitationRequest ReceiveInvitationRequest;

//    public Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      ReceiveInvitationRequest = new ReceiveInvitationRequest { Days = 10 };
//    }

//    public async Task ReceiveInvitationResponse()
//    {
//      ReceiveInvitationResponse ReceiveInvitationResponse =
//        await GetJsonAsync<ReceiveInvitationResponse>(ReceiveInvitationRequest.GetRoute());

//      ValidateReceiveInvitationResponse(ReceiveInvitationResponse);
//    }

//    public async Task ValidationError()
//    {
//      // Set invalid value
//      ReceiveInvitationRequest.Days = -1;

//      HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(ReceiveInvitationRequest.GetRoute());

//      string json = await httpResponseMessage.Content.ReadAsStringAsync();

//      httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//      json.Should().Contain("errors");
//      json.Should().Contain(nameof(ReceiveInvitationRequest.Days));
//    }

//    private void ValidateReceiveInvitationResponse(ReceiveInvitationResponse aReceiveInvitationResponse)
//    {
//      aReceiveInvitationResponse.CorrelationId.Should().Be(ReceiveInvitationRequest.CorrelationId);
//      // check Other properties here
//    }
//  }
//}