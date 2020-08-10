//namespace GetCredentialsForProofHandler
//{
//  using System.Threading.Tasks;
//  using System.Text.Json;
//  using Microsoft.AspNetCore.Mvc.Testing;
//  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
//  using Hyperledger.Aries.AspNetCore.Features.PresentProofs;
//  using Hyperledger.Aries.AspNetCore.Server;
//  using FluentAssertions;

//  public class Handle_Returns : BaseTest
//  {
//    private readonly GetCredentialsForProofRequest GetCredentialsForProofRequest;

//    public Handle_Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      GetCredentialsForProofRequest = new GetCredentialsForProofRequest { Days = 10 };
//    }

//    public async Task GetCredentialsForProofResponse()
//    {
//      GetCredentialsForProofResponse GetCredentialsForProofResponse = await Send(GetCredentialsForProofRequest);

//      ValidateGetCredentialsForProofResponse(GetCredentialsForProofResponse);
//    }

//    private void ValidateGetCredentialsForProofResponse(GetCredentialsForProofResponse aGetCredentialsForProofResponse)
//    {
//      aGetCredentialsForProofResponse.CorrelationId.Should().Be(GetCredentialsForProofRequest.CorrelationId);
//      // check Other properties here
//    }

//  }
//}