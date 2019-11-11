using System;
using AgentFramework.Core.Configuration;
using AgentFramework.Core.Configuration.Options;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Handlers;
using AgentFramework.Core.Handlers.Agents;
using AgentFramework.Core.Handlers.Hosting;
using AgentFramework.Core.Models.Wallets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

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
