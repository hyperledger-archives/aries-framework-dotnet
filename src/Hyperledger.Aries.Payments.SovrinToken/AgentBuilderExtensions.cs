using System;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Payments;
using Hyperledger.Aries.Payments.SovrinToken;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Agent Builder Extensions
    /// </summary>
    public static class AgentBuilderExtensions
    {
        /// <summary>
        /// Add support for payments with Sovrin Token
        /// </summary>
        /// <param name="agentFrameworkBuilder"></param>
        /// <returns></returns>
        public static AriesFrameworkBuilder AddSovrinToken(this AriesFrameworkBuilder agentFrameworkBuilder)
        {
            agentFrameworkBuilder.Services.AddHostedService<SovrinTokenConfigurationService>();
            agentFrameworkBuilder.Services.AddSingleton<IPaymentService, SovrinPaymentService>();
            agentFrameworkBuilder.Services.AddSingleton<IAgentMiddleware, PaymentsAgentMiddleware>();
            return agentFrameworkBuilder;
        }

        /// <summary>
        /// Confgures the default payment address by adding support for determinstic address generation
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static AriesFrameworkBuilder ConfigurePaymentAddress(this AriesFrameworkBuilder builder, Action<AddressOptions> options)
        {
            builder.Services.Configure<AddressOptions>(options);
            return builder;
        }
    }
}
