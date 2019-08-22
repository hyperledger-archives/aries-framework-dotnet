using System.Diagnostics;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebAgent.Models;

namespace WebAgent.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWalletService _walletService;
        private readonly IProvisioningService _provisioningService;
        private readonly WalletOptions _walletOptions;

        public HomeController(
            IWalletService walletService,
            IProvisioningService provisioningService,
            IOptions<WalletOptions> walletOptions)
        {
            _walletService = walletService;
            _provisioningService = provisioningService;
            _walletOptions = walletOptions.Value;
        }

        public async Task<IActionResult> Index()
        {
            var wallet = await _walletService.GetWalletAsync(
                _walletOptions.WalletConfiguration,
                _walletOptions.WalletCredentials);

            var provisioning = await _provisioningService.GetProvisioningAsync(wallet);
            return View(provisioning);
        }
        
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
