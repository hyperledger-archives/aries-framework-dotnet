//namespace GetCredentialHandler
//{
//  using System.Threading.Tasks;
//  using System.Text.Json;
//  using Microsoft.AspNetCore.Mvc.Testing;
//  using Hyperledger.Aries.OpenApi.Server.Integration.Tests.Infrastructure;
//  using Hyperledger.Aries.OpenApi.Features.Credentials;
//  using Hyperledger.Aries.OpenApi.Server;
//  using FluentAssertions;

//  public class Handle_Returns : BaseTest
//  {
//    private readonly GetCredentialRequest GetCredentialRequest;

//    public Handle_Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      GetCredentialRequest = new GetCredentialRequest { Days = 10 };
//    }

//    public async Task GetCredentialResponse()
//    {
//      GetCredentialResponse GetCredentialResponse = await Send(GetCredentialRequest);

//      ValidateGetCredentialResponse(GetCredentialResponse);
//    }

//    private void ValidateGetCredentialResponse(GetCredentialResponse aGetCredentialResponse)
//    {
//      aGetCredentialResponse.CorrelationId.Should().Be(GetCredentialRequest.CorrelationId);
//      // check Other properties here
//    }

//  }
//}