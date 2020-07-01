namespace TestHelpers
{
  using BlazorHosted.Features.Wallets;
  using FluentAssertions;

  internal static class ResetWalletTestHelper
  {
    internal static void ValidateResetWalletResponse(GetWalletRequest aGetWalletRequest, GetWalletResponse aGetWalletResponse)
    {
      aGetWalletResponse.CorrelationId.Should().Be(aGetWalletRequest.CorrelationId);
    }
  }
}
