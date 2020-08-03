//namespace GetProofsEndpoint
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
//    private readonly GetProofsRequest GetProofsRequest;

//    public Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      GetProofsRequest = new GetProofsRequest { Days = 10 };
//    }

//    public async Task GetProofsResponse()
//    {
//      GetProofsResponse GetProofsResponse =
//        await GetJsonAsync<GetProofsResponse>(GetProofsRequest.RouteFactory);

//      ValidateGetProofsResponse(GetProofsResponse);
//    }

//    public async Task ValidationError()
//    {
//      // Set invalid value
//      GetProofsRequest.Days = -1;

//      HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(GetProofsRequest.RouteFactory);

//      string json = await httpResponseMessage.Content.ReadAsStringAsync();

//      httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//      json.Should().Contain("errors");
//      json.Should().Contain(nameof(GetProofsRequest.Days));
//    }

//    private void ValidateGetProofsResponse(GetProofsResponse aGetProofsResponse)
//    {
//      aGetProofsResponse.CorrelationId.Should().Be(GetProofsRequest.CorrelationId);
//      // check Other properties here
//    }
//  }
//}