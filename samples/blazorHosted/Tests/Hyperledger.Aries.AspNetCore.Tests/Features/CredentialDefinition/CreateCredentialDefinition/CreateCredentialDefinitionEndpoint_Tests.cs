//namespace CreateCredentialDefinitionEndpoint
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
//    private readonly CreateCredentialDefinitionRequest CreateCredentialDefinitionRequest;

//    public Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      CreateCredentialDefinitionRequest = new CreateCredentialDefinitionRequest { Days = 10 };
//    }

//    public async Task CreateCredentialDefinitionResponse()
//    {
//      CreateCredentialDefinitionResponse CreateCredentialDefinitionResponse =
//        await GetJsonAsync<CreateCredentialDefinitionResponse>(CreateCredentialDefinitionRequest.GetRoute());

//      ValidateCreateCredentialDefinitionResponse(CreateCredentialDefinitionResponse);
//    }

//    public async Task ValidationError()
//    {
//      // Set invalid value
//      CreateCredentialDefinitionRequest.Days = -1;

//      HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(CreateCredentialDefinitionRequest.GetRoute());

//      string json = await httpResponseMessage.Content.ReadAsStringAsync();

//      httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//      json.Should().Contain("errors");
//      json.Should().Contain(nameof(CreateCredentialDefinitionRequest.Days));
//    }

//    private void ValidateCreateCredentialDefinitionResponse(CreateCredentialDefinitionResponse aCreateCredentialDefinitionResponse)
//    {
//      aCreateCredentialDefinitionResponse.CorrelationId.Should().Be(CreateCredentialDefinitionRequest.CorrelationId);
//      // check Other properties here
//    }
//  }
//}