namespace BlazorHosted.Server.Integration.Tests.Infrastructure
{
  using BlazorHosted.Features.Wallets;
  using FluentAssertions;
  using System.Threading.Tasks;

  public partial class BaseTest
  {
    internal static void ValidateResetWalletResponse(ResetWalletRequest aResetWalletRequest, ResetWalletResponse aResetWalletResponse)
    {
      aResetWalletResponse.CorrelationId.Should().Be(aResetWalletRequest.CorrelationId);
    }

    internal Task ResetAgent() => Send(new ResetWalletRequest());
  }
}
