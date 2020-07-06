//namespace AcceptProofRequestEndpoint
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
//    private readonly AcceptProofRequestRequest AcceptProofRequestRequest;

//    public Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      AcceptProofRequestRequest = new AcceptProofRequestRequest { Days = 10 };
//    }

//    public async Task AcceptProofRequestResponse()
//    {
//      AcceptProofRequestResponse AcceptProofRequestResponse =
//        await GetJsonAsync<AcceptProofRequestResponse>(AcceptProofRequestRequest.RouteFactory);

//      ValidateAcceptProofRequestResponse(AcceptProofRequestResponse);
//    }

//    public async Task ValidationError()
//    {
//      // Set invalid value
//      AcceptProofRequestRequest.Days = -1;

//      HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(AcceptProofRequestRequest.RouteFactory);

//      string json = await httpResponseMessage.Content.ReadAsStringAsync();

//      httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//      json.Should().Contain("errors");
//      json.Should().Contain(nameof(AcceptProofRequestRequest.Days));
//    }

//    private void ValidateAcceptProofRequestResponse(AcceptProofRequestResponse aAcceptProofRequestResponse)
//    {
//      aAcceptProofRequestResponse.CorrelationId.Should().Be(AcceptProofRequestRequest.CorrelationId);
//      // check Other properties here
//    }
//  }
//}