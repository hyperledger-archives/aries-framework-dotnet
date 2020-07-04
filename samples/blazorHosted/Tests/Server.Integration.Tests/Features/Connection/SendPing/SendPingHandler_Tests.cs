//namespace SendPingHandler
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
//    private readonly SendPingRequest SendPingRequest;

//    public Handle_Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      SendPingRequest = new SendPingRequest { Days = 10 };
//    }

//    public async Task SendPingResponse()
//    {
//      SendPingResponse SendPingResponse = await Send(SendPingRequest);

//      ValidateSendPingResponse(SendPingResponse);
//    }

//    private void ValidateSendPingResponse(SendPingResponse aSendPingResponse)
//    {
//      aSendPingResponse.CorrelationId.Should().Be(SendPingRequest.CorrelationId);
//      // check Other properties here
//    }

//  }
//}