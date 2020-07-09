//namespace GetProofEndpoint
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
//    private readonly GetProofRequest GetProofRequest;

//    public Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      GetProofRequest = new GetProofRequest { Days = 10 };
//    }

//    public async Task GetProofResponse()
//    {
//      GetProofResponse GetProofResponse =
//        await GetJsonAsync<GetProofResponse>(GetProofRequest.RouteFactory);

//      ValidateGetProofResponse(GetProofResponse);
//    }

//    public async Task ValidationError()
//    {
//      // Set invalid value
//      GetProofRequest.Days = -1;

//      HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(GetProofRequest.RouteFactory);

//      string json = await httpResponseMessage.Content.ReadAsStringAsync();

//      httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//      json.Should().Contain("errors");
//      json.Should().Contain(nameof(GetProofRequest.Days));
//    }

//    private void ValidateGetProofResponse(GetProofResponse aGetProofResponse)
//    {
//      aGetProofResponse.CorrelationId.Should().Be(GetProofRequest.CorrelationId);
//      // check Other properties here
//    }
//  }
//}