//namespace SendPingHandler
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