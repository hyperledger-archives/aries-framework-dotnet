//namespace GetConnectionEndpoint
//{
//  using FluentAssertions;
//  using Microsoft.AspNetCore.Mvc.Testing;
//  using System.Net;
//  using System.Net.Http;
//  using System.Text.Json;
//  using System.Threading.Tasks;
//  using BlazorHosted.Features.Connections;
//  using BlazorHosted.Server.Integration.Tests.Infrastructure;
//  using BlazorHosted.Server;

//  public class Returns : BaseTest
//  {
//    private readonly GetConnectionRequest GetConnectionRequest;

//    public Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      GetConnectionRequest = new GetConnectionRequest { Days = 10 };
//    }

//    public async Task GetConnectionResponse()
//    {
//      GetConnectionResponse GetConnectionResponse =
//        await GetJsonAsync<GetConnectionResponse>(GetConnectionRequest.GetRoute());

//      ValidateGetConnectionResponse(GetConnectionResponse);
//    }

//    public async Task ValidationError()
//    {
//      // Set invalid value
//      GetConnectionRequest.Days = -1;

//      HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(GetConnectionRequest.GetRoute());

//      string json = await httpResponseMessage.Content.ReadAsStringAsync();

//      httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//      json.Should().Contain("errors");
//      json.Should().Contain(nameof(GetConnectionRequest.Days));
//    }

//    private void ValidateGetConnectionResponse(GetConnectionResponse aGetConnectionResponse)
//    {
//      aGetConnectionResponse.CorrelationId.Should().Be(GetConnectionRequest.CorrelationId);
//      // check Other properties here
//    }
//  }
//}