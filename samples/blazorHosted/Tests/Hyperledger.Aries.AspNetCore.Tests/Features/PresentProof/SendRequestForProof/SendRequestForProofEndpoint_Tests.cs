//namespace SendRequestForProofEndpoint
//{
//  using FluentAssertions;
//  using Microsoft.AspNetCore.Mvc.Testing;
//  using System.Net;
//  using System.Net.Http;
//  using System.Text.Json;
//  using System.Threading.Tasks;
//  using Hyperledger.Aries.AspNetCore.Features.PresentProofs;
//  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
//  using Hyperledger.Aries.AspNetCore.Server;

//  public class Returns : BaseTest
//  {
//    private readonly SendRequestForProofRequest SendRequestForProofRequest;

//    public Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      SendRequestForProofRequest = new SendRequestForProofRequest { Days = 10 };
//    }

//    public async Task SendRequestForProofResponse()
//    {
//      SendRequestForProofResponse SendRequestForProofResponse =
//        await GetJsonAsync<SendRequestForProofResponse>(SendRequestForProofRequest.RouteFactory);

//      ValidateSendRequestForProofResponse(SendRequestForProofResponse);
//    }

//    public async Task ValidationError()
//    {
//      // Set invalid value
//      SendRequestForProofRequest.Days = -1;

//      HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(SendRequestForProofRequest.RouteFactory);

//      string json = await httpResponseMessage.Content.ReadAsStringAsync();

//      httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//      json.Should().Contain("errors");
//      json.Should().Contain(nameof(SendRequestForProofRequest.Days));
//    }

//    private void ValidateSendRequestForProofResponse(SendRequestForProofResponse aSendRequestForProofResponse)
//    {
//      aSendRequestForProofResponse.CorrelationId.Should().Be(SendRequestForProofRequest.CorrelationId);
//      // check Other properties here
//    }
//  }
//}