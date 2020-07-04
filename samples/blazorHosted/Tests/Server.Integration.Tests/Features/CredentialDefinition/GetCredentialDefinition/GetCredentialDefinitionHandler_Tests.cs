//namespace GetCredentialDefinitionHandler
//{
//  using System.Threading.Tasks;
//  using System.Text.Json;
//  using Microsoft.AspNetCore.Mvc.Testing;
//  using Hyperledger.Aries.OpenApi.Server.Integration.Tests.Infrastructure;
//  using Hyperledger.Aries.OpenApi.Features.CredentialDefinitions;
//  using Hyperledger.Aries.OpenApi.Server;
//  using FluentAssertions;

//  public class Handle_Returns : BaseTest
//  {
//    private readonly GetCredentialDefinitionRequest GetCredentialDefinitionRequest;

//    public Handle_Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      GetCredentialDefinitionRequest = new GetCredentialDefinitionRequest { Days = 10 };
//    }

//    public async Task GetCredentialDefinitionResponse()
//    {
//      GetCredentialDefinitionResponse GetCredentialDefinitionResponse = await Send(GetCredentialDefinitionRequest);

//      ValidateGetCredentialDefinitionResponse(GetCredentialDefinitionResponse);
//    }

//    private void ValidateGetCredentialDefinitionResponse(GetCredentialDefinitionResponse aGetCredentialDefinitionResponse)
//    {
//      aGetCredentialDefinitionResponse.CorrelationId.Should().Be(GetCredentialDefinitionRequest.CorrelationId);
//      // check Other properties here
//    }

//  }
//}