using System;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Agent builder extensions.
    /// </summary>
    public static class AgentFrameworkBuilderExtensions
    {
        /// <summary>
        /// Registers and provisions an agent.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static AriesFrameworkBuilder RegisterAgent(
            this AriesFrameworkBuilder builder,
            Action<AgentOptions> options) 
            => RegisterAgent<DefaultAgent>(builder, options);

        /// <summary>
        /// Registers and provisions an agent with custom implementation
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static AriesFrameworkBuilder RegisterAgent<T>(
            this AriesFrameworkBuilder builder, 
            Action<AgentOptions> options)
            where T : class, IAgent 
        {
            builder.AddAgentProvider();
            builder.Services.AddDefaultMessageHandlers();
            builder.Services.AddSingleton<IAgent, T>();
            builder.Services.Configure(options);
            builder.Services.AddHostedService<DefaultProvisioningHostedService>();
            builder.Services.AddHostedService<PoolConfigurationService>();

            return builder;
        }

        /// <summary>
        /// Accepts the latest transaction author agreement on service startup
        /// and stores the configuration in the <see cref="ProvisioningRecord" />.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static AriesFrameworkBuilder AcceptTxnAuthorAgreement(this AriesFrameworkBuilder builder)
        {
            builder.Services.AddHostedService<TxnAuthorAcceptanceService>();
            return builder;
        }

        /// <summary>
        /// Adds agent provider services
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static AriesFrameworkBuilder AddAgentProvider(this AriesFrameworkBuilder builder)
        {
            builder.Services.AddSingleton<IAgentProvider, DefaultAgentProvider>();
            return builder;
        }
    }
}
