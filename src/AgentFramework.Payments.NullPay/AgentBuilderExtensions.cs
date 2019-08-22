using AgentFramework.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AgentFramework.Payments.NullPay
{
    public static class AgentBuilderExtensions
    {
        public static AgentFrameworkBuilder AddNullPay(this AgentFrameworkBuilder agentFrameworkBuilder)
        {
            agentFrameworkBuilder.Services.AddHostedService<NullPayConfigurationService>();
            return agentFrameworkBuilder;
        }
    }
}
