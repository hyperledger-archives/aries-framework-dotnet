using System;
using AgentFramework.Core.Configuration;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Handlers.Agents;
using AgentFramework.Core.Models.Payments;
using AgentFramework.Payments.SovrinToken;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AgentBuilderExtensions
    {
        public static AriesFrameworkBuilder AddSovrinToken(this AriesFrameworkBuilder agentFrameworkBuilder)
        {
            agentFrameworkBuilder.Services.AddHostedService<SovrinTokenConfigurationService>();
            agentFrameworkBuilder.Services.AddSingleton<IPaymentService, SovrinPaymentService>();
            agentFrameworkBuilder.Services.AddSingleton<IAgentMiddleware, PaymentsAgentMiddleware>();
            return agentFrameworkBuilder;
        }

        public static AriesFrameworkBuilder ConfigurePaymentAddress(this AriesFrameworkBuilder builder, Action<AddressOptions> options)
        {
            builder.Services.Configure<AddressOptions>(options);
            return builder;
        }
    }
}
