//namespace GetHealthEndpoint
//{
//  using FluentAssertions;
//  using Microsoft.AspNetCore.Mvc.Testing;
//  using System.Net;
//  using System.Net.Http;
//  using System.Text.Json;
//  using System.Threading.Tasks;
//  using Hyperledger.Aries.AspNetCore.Features.Healths;
//  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
//  using Hyperledger.Aries.AspNetCore.Server;

//  public class Returns : BaseTest
//  {
//    private readonly GetHealthRequest GetHealthRequest;

//    public Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      GetHealthRequest = new GetHealthRequest { Days = 10 };
//    }

//    public async Task GetHealthResponse()
//    {
//      GetHealthResponse GetHealthResponse =
//        await GetJsonAsync<GetHealthResponse>(GetHealthRequest.GetRoute());

//      ValidateGetHealthResponse(GetHealthResponse);
//    }

//    public async Task ValidationError()
//    {
//      // Set invalid value
//      GetHealthRequest.Days = -1;

//      HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(GetHealthRequest.GetRoute());

//      string json = await httpResponseMessage.Content.ReadAsStringAsync();

//      httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//      json.Should().Contain("errors");
//      json.Should().Contain(nameof(GetHealthRequest.Days));
//    }

//    private void ValidateGetHealthResponse(GetHealthResponse aGetHealthResponse)
//    {
//      aGetHealthResponse.CorrelationId.Should().Be(GetHealthRequest.CorrelationId);
//      // check Other properties here
//    }
//  }
//}