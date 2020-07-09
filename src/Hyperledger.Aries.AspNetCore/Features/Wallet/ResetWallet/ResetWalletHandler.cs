namespace Hyperledger.Aries.AspNetCore.Features.Wallets
{
  using Hyperledger.Aries.Configuration;
  using Hyperledger.Aries.Storage;
  using Hyperledger.Indy.WalletApi;
  using MediatR;
  using Microsoft.Extensions.Options;
  using System.Threading;
  using System.Threading.Tasks;

  public class ResetWalletHandler : IRequestHandler<ResetWalletRequest, ResetWalletResponse>
  {
    private readonly AgentOptions AgentOptions;
    private readonly IProvisioningService ProvisioningService;
    private readonly IWalletService WalletService;

    public ResetWalletHandler
    (
      IWalletService aWalletService,
      IProvisioningService aProvisioningService,
      IOptions<AgentOptions> aAgentOptions
    )
    {
      WalletService = aWalletService;
      ProvisioningService = aProvisioningService;
      AgentOptions = aAgentOptions.Value;
    }

    public async Task<ResetWalletResponse> Handle
    (
      ResetWalletRequest aResetWalletRequest,
      CancellationToken aCancellationToken
    )
    {
      await WalletService.DeleteWalletAsync(AgentOptions.WalletConfiguration, AgentOptions.WalletCredentials);

      await ProvisioningService.ProvisionAgentAsync();

      var response = new ResetWalletResponse(aResetWalletRequest.CorrelationId);

      return response;
    }
  }
}
