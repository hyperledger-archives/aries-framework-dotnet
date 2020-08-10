namespace ResetWalletHandler
{
  using Hyperledger.Aries.AspNetCore.Features.Connections;
  using Hyperledger.Aries.AspNetCore.Features.Wallets;
  using Hyperledger.Aries.AspNetCore.Server;
  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
  using FluentAssertions;
  using Microsoft.AspNetCore.Mvc.Testing;
  using Newtonsoft.Json;
  using System.Threading.Tasks;  

  public class Handle_Returns : BaseTest
  {
    private readonly ResetWalletRequest ResetWalletRequest;

    public Handle_Returns
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerSettings aJsonSerializerSettings
    ) : base(aWebApplicationFactory, aJsonSerializerSettings)
    {
      ResetWalletRequest = new ResetWalletRequest();
    }

    public async Task ResetWalletResponse_and_reset_wallet()
    {
      // Arrage
      // Add something to the wallet 
      await CreateAnInvitation();

      //Act
      ResetWalletResponse resetWalletResponse = await Send(ResetWalletRequest);

      // See what is in the wallet
      GetConnectionsRequest getConnectionsRequest = CreateValidGetConnectionsRequest();
      GetConnectionsResponse getConnectionsResponse = await Send(getConnectionsRequest);

      // Assert Item created isn't there
      getConnectionsResponse.ConnectionRecords.Count.Should().Be(0);

      ValidateResetWalletResponse(ResetWalletRequest, resetWalletResponse);
    }
  }
}
