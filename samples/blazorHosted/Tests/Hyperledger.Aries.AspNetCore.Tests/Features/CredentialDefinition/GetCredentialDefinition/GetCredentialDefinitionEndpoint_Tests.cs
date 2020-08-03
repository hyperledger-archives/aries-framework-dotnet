//namespace GetCredentialDefinitionEndpoint
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
//    private readonly GetCredentialDefinitionRequest GetCredentialDefinitionRequest;

//    public Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      GetCredentialDefinitionRequest = new GetCredentialDefinitionRequest { Days = 10 };
//    }

//    public async Task GetCredentialDefinitionResponse()
//    {
//      GetCredentialDefinitionResponse GetCredentialDefinitionResponse =
//        await GetJsonAsync<GetCredentialDefinitionResponse>(GetCredentialDefinitionRequest.GetRoute());

//      ValidateGetCredentialDefinitionResponse(GetCredentialDefinitionResponse);
//    }

//    public async Task ValidationError()
//    {
//      // Set invalid value
//      GetCredentialDefinitionRequest.Days = -1;

//      HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(GetCredentialDefinitionRequest.GetRoute());

//      string json = await httpResponseMessage.Content.ReadAsStringAsync();

//      httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//      json.Should().Contain("errors");
//      json.Should().Contain(nameof(GetCredentialDefinitionRequest.Days));
//    }

//    private void ValidateGetCredentialDefinitionResponse(GetCredentialDefinitionResponse aGetCredentialDefinitionResponse)
//    {
//      aGetCredentialDefinitionResponse.CorrelationId.Should().Be(GetCredentialDefinitionRequest.CorrelationId);
//      // check Other properties here
//    }
//  }
//}