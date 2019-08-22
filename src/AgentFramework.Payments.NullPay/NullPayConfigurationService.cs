using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace AgentFramework.Payments.NullPay
{
    internal class NullPayConfigurationService : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Configuration.InitializeAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}