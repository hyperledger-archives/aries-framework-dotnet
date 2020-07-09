//namespace GetSchemaEndpoint
//{
//  using FluentAssertions;
//  using Microsoft.AspNetCore.Mvc.Testing;
//  using System.Net;
//  using System.Net.Http;
//  using System.Text.Json;
//  using System.Threading.Tasks;
//  using Hyperledger.Aries.AspNetCore.Features.Schemas;
//  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
//  using Hyperledger.Aries.AspNetCore.Server;

//  public class Returns : BaseTest
//  {
//    private readonly GetSchemaRequest GetSchemaRequest;

//    public Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      GetSchemaRequest = new GetSchemaRequest { Days = 10 };
//    }

//    public async Task GetSchemaResponse()
//    {
//      GetSchemaResponse GetSchemaResponse =
//        await GetJsonAsync<GetSchemaResponse>(GetSchemaRequest.GetRoute());

//      ValidateGetSchemaResponse(GetSchemaResponse);
//    }

//    public async Task ValidationError()
//    {
//      // Set invalid value
//      GetSchemaRequest.Days = -1;

//      HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(GetSchemaRequest.GetRoute());

//      string json = await httpResponseMessage.Content.ReadAsStringAsync();

//      httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//      json.Should().Contain("errors");
//      json.Should().Contain(nameof(GetSchemaRequest.Days));
//    }

//    private void ValidateGetSchemaResponse(GetSchemaResponse aGetSchemaResponse)
//    {
//      aGetSchemaResponse.CorrelationId.Should().Be(GetSchemaRequest.CorrelationId);
//      // check Other properties here
//    }
//  }
//}