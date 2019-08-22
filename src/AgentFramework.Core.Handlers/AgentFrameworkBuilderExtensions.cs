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
        /// Adds a <see cref="DefaultAgent"/> service provisioned with <see cref="BasicProvisioningConfiguration"/>
        /// </summary>
        /// <returns>The basic agent.</returns>
        /// <param name="frameworkBuilder">Builder.</param>
        /// <param name="config">Config.</param>
        public static AgentFrameworkBuilder AddBasicAgent(this AgentFrameworkBuilder frameworkBuilder, Action<BasicProvisioningConfiguration> config)
        {
            return AddBasicAgent<DefaultAgent>(frameworkBuilder, config);
        }

        /// <summary>
        /// Adds a custom agent service provisioned with <see cref="BasicProvisioningConfiguration"/>
        /// </summary>
        /// <returns>The basic agent.</returns>
        /// <param name="frameworkBuilder">Builder.</param>
        /// <param name="config">Config.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static AgentFrameworkBuilder AddBasicAgent<T>(this AgentFrameworkBuilder frameworkBuilder, Action<BasicProvisioningConfiguration> config)
            where T : class, IAgent
        {
            var configuration = new BasicProvisioningConfiguration();
            config?.Invoke(configuration);

            return AddAgent<T, BasicProvisioningConfiguration>(frameworkBuilder, () => configuration);
        }

        /// <summary>
        /// Adds a <see cref="DefaultAgent"/> service provisioned with <see cref="IssuerProvisioningConfiguration"/>
        /// </summary>
        /// <returns>The issuer agent.</returns>
        /// <param name="frameworkBuilder">Builder.</param>
        /// <param name="config">Config.</param>
        public static AgentFrameworkBuilder AddIssuerAgent(this AgentFrameworkBuilder frameworkBuilder, Action<IssuerProvisioningConfiguration> config)
        {
            return AddIssuerAgent<DefaultAgent>(frameworkBuilder, config);
        }

        /// <summary>
        /// Adds a custom agent service provisioned with <see cref="IssuerProvisioningConfiguration"/>
        /// </summary>
        /// <returns>The issuer agent.</returns>
        /// <param name="frameworkBuilder">Builder.</param>
        /// <param name="config">Config.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static AgentFrameworkBuilder AddIssuerAgent<T>(this AgentFrameworkBuilder frameworkBuilder, Action<IssuerProvisioningConfiguration> config)
            where T : class, IAgent
        {
            var configuration = new IssuerProvisioningConfiguration();
            config?.Invoke(configuration);

            return AddAgent<T, IssuerProvisioningConfiguration>(frameworkBuilder, () => configuration);
        }

        /// <summary>
        /// Adds a custom agent service provisioned with custom <see cref="ProvisioningConfiguration"/>
        /// </summary>
        /// <returns>The agent.</returns>
        /// <param name="frameworkBuilder">Builder.</param>
        /// <param name="createConfiguration">Create configuration.</param>
        /// <typeparam name="TAgent">The 1st type parameter.</typeparam>
        /// <typeparam name="TConfiguration">The 2nd type parameter.</typeparam>
        public static AgentFrameworkBuilder AddAgent<TAgent, TConfiguration>(this AgentFrameworkBuilder frameworkBuilder, Func<TConfiguration> createConfiguration)
            where TAgent : class, IAgent
            where TConfiguration : ProvisioningConfiguration
        {
            var configuration = createConfiguration.Invoke();

            frameworkBuilder.Services.Configure<WalletOptions>(obj =>
            {
                obj.WalletConfiguration = configuration.WalletConfiguration;
                obj.WalletCredentials = configuration.WalletCredentials;
            });

            frameworkBuilder.Services.Configure<PoolOptions>(obj =>
            {
                obj.PoolName = configuration.PoolName;
                obj.GenesisFilename = configuration.GenesisFilename;
            });

            frameworkBuilder.AddAgentProvider();
            frameworkBuilder.Services.AddDefaultMessageHandlers();
            frameworkBuilder.Services.AddSingleton<ProvisioningConfiguration>(configuration);
            frameworkBuilder.Services.AddSingleton<IAgent, TAgent>();
            frameworkBuilder.Services.AddSingleton<IHostedService>(s => new ProvisioningHostedService(
                configuration,
                s.GetRequiredService<IProvisioningService>(),
                s.GetRequiredService<IPoolService>(),
                s.GetRequiredService<IOptions<PoolOptions>>()));

            return frameworkBuilder;
        }

        /// <summary>
        /// Adds agent provider services
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static AgentFrameworkBuilder AddAgentProvider(this AgentFrameworkBuilder builder)
        {
            builder.Services.AddSingleton<IAgentProvider, DefaultAgentProvider>();
            return builder;
        }
    }
}
