//namespace CreateSchemaEndpoint
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
//    private readonly CreateSchemaRequest CreateSchemaRequest;

//    public Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      CreateSchemaRequest = new CreateSchemaRequest { Days = 10 };
//    }

//    public async Task CreateSchemaResponse()
//    {
//      CreateSchemaResponse CreateSchemaResponse =
//        await GetJsonAsync<CreateSchemaResponse>(CreateSchemaRequest.GetRoute());

//      ValidateCreateSchemaResponse(CreateSchemaResponse);
//    }

//    public async Task ValidationError()
//    {
//      // Set invalid value
//      CreateSchemaRequest.Days = -1;

//      HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(CreateSchemaRequest.GetRoute());

//      string json = await httpResponseMessage.Content.ReadAsStringAsync();

//      httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//      json.Should().Contain("errors");
//      json.Should().Contain(nameof(CreateSchemaRequest.Days));
//    }

//    private void ValidateCreateSchemaResponse(CreateSchemaResponse aCreateSchemaResponse)
//    {
//      aCreateSchemaResponse.CorrelationId.Should().Be(CreateSchemaRequest.CorrelationId);
//      // check Other properties here
//    }
//  }
//}