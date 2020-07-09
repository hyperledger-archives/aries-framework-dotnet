//namespace GetCredentialsForProofEndpoint
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
//    private readonly GetCredentialsForProofRequest GetCredentialsForProofRequest;

//    public Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      GetCredentialsForProofRequest = new GetCredentialsForProofRequest { Days = 10 };
//    }

//    public async Task GetCredentialsForProofResponse()
//    {
//      GetCredentialsForProofResponse GetCredentialsForProofResponse =
//        await GetJsonAsync<GetCredentialsForProofResponse>(GetCredentialsForProofRequest.RouteFactory);

//      ValidateGetCredentialsForProofResponse(GetCredentialsForProofResponse);
//    }

//    public async Task ValidationError()
//    {
//      // Set invalid value
//      GetCredentialsForProofRequest.Days = -1;

//      HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(GetCredentialsForProofRequest.RouteFactory);

//      string json = await httpResponseMessage.Content.ReadAsStringAsync();

//      httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//      json.Should().Contain("errors");
//      json.Should().Contain(nameof(GetCredentialsForProofRequest.Days));
//    }

//    private void ValidateGetCredentialsForProofResponse(GetCredentialsForProofResponse aGetCredentialsForProofResponse)
//    {
//      aGetCredentialsForProofResponse.CorrelationId.Should().Be(GetCredentialsForProofRequest.CorrelationId);
//      // check Other properties here
//    }
//  }
//}