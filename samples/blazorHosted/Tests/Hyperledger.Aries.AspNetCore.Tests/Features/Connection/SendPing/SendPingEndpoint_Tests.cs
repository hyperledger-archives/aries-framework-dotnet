//namespace SendPingEndpoint
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
//    private readonly SendPingRequest SendPingRequest;

//    public Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      SendPingRequest = new SendPingRequest { Days = 10 };
//    }

//    public async Task SendPingResponse()
//    {
//      SendPingResponse SendPingResponse =
//        await GetJsonAsync<SendPingResponse>(SendPingRequest.GetRoute());

//      ValidateSendPingResponse(SendPingResponse);
//    }

//    public async Task ValidationError()
//    {
//      // Set invalid value
//      SendPingRequest.Days = -1;

//      HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(SendPingRequest.GetRoute());

//      string json = await httpResponseMessage.Content.ReadAsStringAsync();

//      httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//      json.Should().Contain("errors");
//      json.Should().Contain(nameof(SendPingRequest.Days));
//    }

//    private void ValidateSendPingResponse(SendPingResponse aSendPingResponse)
//    {
//      aSendPingResponse.CorrelationId.Should().Be(SendPingRequest.CorrelationId);
//      // check Other properties here
//    }
//  }
//}