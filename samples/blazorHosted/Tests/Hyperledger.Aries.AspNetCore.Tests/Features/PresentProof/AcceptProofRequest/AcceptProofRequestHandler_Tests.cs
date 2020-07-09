//namespace AcceptProofRequestHandler
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
//    private readonly AcceptProofRequestRequest AcceptProofRequestRequest;

//    public Handle_Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      AcceptProofRequestRequest = new AcceptProofRequestRequest { Days = 10 };
//    }

//    public async Task AcceptProofRequestResponse()
//    {
//      AcceptProofRequestResponse AcceptProofRequestResponse = await Send(AcceptProofRequestRequest);

//      ValidateAcceptProofRequestResponse(AcceptProofRequestResponse);
//    }

//    private void ValidateAcceptProofRequestResponse(AcceptProofRequestResponse aAcceptProofRequestResponse)
//    {
//      aAcceptProofRequestResponse.CorrelationId.Should().Be(AcceptProofRequestRequest.CorrelationId);
//      // check Other properties here
//    }

//  }
//}