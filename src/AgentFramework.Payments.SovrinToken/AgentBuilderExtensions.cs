using AgentFramework.Core.Configuration;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Handlers.Agents;
using Microsoft.Extensions.DependencyInjection;

namespace AgentFramework.Payments.SovrinToken
{
    public static class AgentBuilderExtensions
    {
        public static AgentFrameworkBuilder AddSovrinToken(this AgentFrameworkBuilder agentFrameworkBuilder)
        {
            agentFrameworkBuilder.Services.AddHostedService<SovrinTokenConfigurationService>();
            agentFrameworkBuilder.Services.AddSingleton<IPaymentService, SovrinPaymentService>();
            agentFrameworkBuilder.Services.AddSingleton<IAgentMiddleware, PaymentsAgentMiddleware>();
            return agentFrameworkBuilder;
        }
    }
}
