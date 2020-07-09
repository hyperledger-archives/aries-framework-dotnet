//namespace GetCredentialDefinitionsEndpoint
//{
//  using FluentAssertions;
//  using Microsoft.AspNetCore.Mvc.Testing;
//  using System.Net;
//  using System.Net.Http;
//  using System.Text.Json;
//  using System.Threading.Tasks;
//  using Hyperledger.Aries.AspNetCore.Features.CredentialDefinitions;
//  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
//  using Hyperledger.Aries.AspNetCore.Server;

//  public class Returns : BaseTest
//  {
//    private readonly GetCredentialDefinitionsRequest GetCredentialDefinitionsRequest;

//    public Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      GetCredentialDefinitionsRequest = new GetCredentialDefinitionsRequest { Days = 10 };
//    }

//    public async Task GetCredentialDefinitionsResponse()
//    {
//      GetCredentialDefinitionsResponse GetCredentialDefinitionsResponse =
//        await GetJsonAsync<GetCredentialDefinitionsResponse>(GetCredentialDefinitionsRequest.GetRoute());

//      ValidateGetCredentialDefinitionsResponse(GetCredentialDefinitionsResponse);
//    }

//    public async Task ValidationError()
//    {
//      // Set invalid value
//      GetCredentialDefinitionsRequest.Days = -1;

//      HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(GetCredentialDefinitionsRequest.GetRoute());

//      string json = await httpResponseMessage.Content.ReadAsStringAsync();

//      httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//      json.Should().Contain("errors");
//      json.Should().Contain(nameof(GetCredentialDefinitionsRequest.Days));
//    }

//    private void ValidateGetCredentialDefinitionsResponse(GetCredentialDefinitionsResponse aGetCredentialDefinitionsResponse)
//    {
//      aGetCredentialDefinitionsResponse.CorrelationId.Should().Be(GetCredentialDefinitionsRequest.CorrelationId);
//      // check Other properties here
//    }
//  }
//}