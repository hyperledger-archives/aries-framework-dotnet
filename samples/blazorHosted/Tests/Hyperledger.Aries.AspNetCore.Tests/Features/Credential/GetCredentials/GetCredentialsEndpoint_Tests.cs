//namespace GetCredentialsEndpoint
//{
//  using FluentAssertions;
//  using Microsoft.AspNetCore.Mvc.Testing;
//  using System.Net;
//  using System.Net.Http;
//  using System.Text.Json;
//  using System.Threading.Tasks;
//  using Hyperledger.Aries.AspNetCore.Features.Credentials;
//  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
//  using Hyperledger.Aries.AspNetCore.Server;

//  public class Returns : BaseTest
//  {
//    private readonly GetCredentialsRequest GetCredentialsRequest;

//    public Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      GetCredentialsRequest = new GetCredentialsRequest { Days = 10 };
//    }

//    public async Task GetCredentialsResponse()
//    {
//      GetCredentialsResponse GetCredentialsResponse =
//        await GetJsonAsync<GetCredentialsResponse>(GetCredentialsRequest.GetRoute());

//      ValidateGetCredentialsResponse(GetCredentialsResponse);
//    }

//    public async Task ValidationError()
//    {
//      // Set invalid value
//      GetCredentialsRequest.Days = -1;

//      HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(GetCredentialsRequest.GetRoute());

//      string json = await httpResponseMessage.Content.ReadAsStringAsync();

//      httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//      json.Should().Contain("errors");
//      json.Should().Contain(nameof(GetCredentialsRequest.Days));
//    }

//    private void ValidateGetCredentialsResponse(GetCredentialsResponse aGetCredentialsResponse)
//    {
//      aGetCredentialsResponse.CorrelationId.Should().Be(GetCredentialsRequest.CorrelationId);
//      // check Other properties here
//    }
//  }
//}