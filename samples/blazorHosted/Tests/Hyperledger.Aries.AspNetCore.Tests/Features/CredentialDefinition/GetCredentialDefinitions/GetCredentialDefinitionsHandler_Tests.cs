//namespace GetCredentialDefinitionsHandler
//{
//  using System.Threading.Tasks;
//  using System.Text.Json;
//  using Microsoft.AspNetCore.Mvc.Testing;
//  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
//  using Hyperledger.Aries.AspNetCore.Features.CredentialDefinitions;
//  using Hyperledger.Aries.AspNetCore.Server;
//  using FluentAssertions;

//  public class Handle_Returns : BaseTest
//  {
//    private readonly GetCredentialDefinitionsRequest GetCredentialDefinitionsRequest;

//    public Handle_Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      GetCredentialDefinitionsRequest = new GetCredentialDefinitionsRequest { Days = 10 };
//    }

//    public async Task GetCredentialDefinitionsResponse()
//    {
//      GetCredentialDefinitionsResponse GetCredentialDefinitionsResponse = await Send(GetCredentialDefinitionsRequest);

//      ValidateGetCredentialDefinitionsResponse(GetCredentialDefinitionsResponse);
//    }

//    private void ValidateGetCredentialDefinitionsResponse(GetCredentialDefinitionsResponse aGetCredentialDefinitionsResponse)
//    {
//      aGetCredentialDefinitionsResponse.CorrelationId.Should().Be(GetCredentialDefinitionsRequest.CorrelationId);
//      // check Other properties here
//    }

//  }
//}