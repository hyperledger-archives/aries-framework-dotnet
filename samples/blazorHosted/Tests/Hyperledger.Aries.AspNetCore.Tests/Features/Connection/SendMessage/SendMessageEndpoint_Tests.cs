//namespace SendMessageEndpoint
//{
//  using FluentAssertions;
//  using Microsoft.AspNetCore.Mvc.Testing;
//  using System.Net;
//  using System.Net.Http;
//  using System.Text.Json;
//  using System.Threading.Tasks;
//  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
//  using Hyperledger.Aries.AspNetCore.Server;
//  using Hyperledger.Aries.AspNetCore.Features.BasicMessaging;

//  public class Returns : BaseTest
//  {
//    private readonly SendMessageRequest SendMessageRequest;

//    public Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      SendMessageRequest = new SendMessageRequest { Message = "Hello World"};
//    }

//    public async Task SendMessageResponse()
//    {
//      SendMessageResponse SendMessageResponse =
//        await GetJsonAsync<SendMessageResponse>(SendMessageRequest.GetRoute());

//      ValidateSendMessageResponse(SendMessageResponse);
//    }

//    public async Task ValidationError()
//    {
//      // Set invalid value
//      SendMessageRequest.Message = string.Empty;

//      HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(SendMessageRequest.GetRoute());

//      string json = await httpResponseMessage.Content.ReadAsStringAsync();

//      httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//      json.Should().Contain("errors");
//      json.Should().Contain(nameof(SendMessageRequest.ConnectionId));
//    }

//    private void ValidateSendMessageResponse(SendMessageResponse aSendMessageResponse)
//    {
//      aSendMessageResponse.CorrelationId.Should().Be(SendMessageRequest.CorrelationId);
//      // check Other properties here
//    }
//  }
//}