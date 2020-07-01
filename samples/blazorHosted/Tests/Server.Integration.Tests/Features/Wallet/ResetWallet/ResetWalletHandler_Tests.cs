//namespace ResetWalletHandler
//{
//  using System.Threading.Tasks;
//  using System.Text.Json;
//  using Microsoft.AspNetCore.Mvc.Testing;
//  using BlazorHosted.Server.Integration.Tests.Infrastructure;
//  using BlazorHosted.Features.Wallets;
//  using BlazorHosted.Server;
//  using FluentAssertions;

//  public class Handle_Returns : BaseTest
//  {
//    private readonly ResetWalletRequest ResetWalletRequest;

//    public Handle_Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      ResetWalletRequest = new ResetWalletRequest { Days = 10 };
//    }

//    public async Task ResetWalletResponse()
//    {
//      ResetWalletResponse ResetWalletResponse = await Send(ResetWalletRequest);

//      ValidateResetWalletResponse(ResetWalletResponse);
//    }

//    private void ValidateResetWalletResponse(ResetWalletResponse aResetWalletResponse)
//    {
//      aResetWalletResponse.CorrelationId.Should().Be(ResetWalletRequest.CorrelationId);
//      // check Other properties here
//    }

//  }
//}