//namespace RecieveInvitationEndpoint
//{
//  using FluentAssertions;
//  using Microsoft.AspNetCore.Mvc.Testing;
//  using System.Net;
//  using System.Net.Http;
//  using System.Text.Json;
//  using System.Threading.Tasks;
//  using BlazorHosted.Features.Connections;
//  using BlazorHosted.Server.Integration.Tests.Infrastructure;
//  using BlazorHosted.Server;

//  public class Returns : BaseTest
//  {
//    private readonly RecieveInvitationRequest RecieveInvitationRequest;

//    public Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      RecieveInvitationRequest = new RecieveInvitationRequest { Days = 10 };
//    }

//    public async Task RecieveInvitationResponse()
//    {
//      RecieveInvitationResponse RecieveInvitationResponse =
//        await GetJsonAsync<RecieveInvitationResponse>(RecieveInvitationRequest.GetRoute());

//      ValidateRecieveInvitationResponse(RecieveInvitationResponse);
//    }

//    public async Task ValidationError()
//    {
//      // Set invalid value
//      RecieveInvitationRequest.Days = -1;

//      HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(RecieveInvitationRequest.GetRoute());

//      string json = await httpResponseMessage.Content.ReadAsStringAsync();

//      httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//      json.Should().Contain("errors");
//      json.Should().Contain(nameof(RecieveInvitationRequest.Days));
//    }

//    private void ValidateRecieveInvitationResponse(RecieveInvitationResponse aRecieveInvitationResponse)
//    {
//      aRecieveInvitationResponse.CorrelationId.Should().Be(RecieveInvitationRequest.CorrelationId);
//      // check Other properties here
//    }
//  }
//}