using System;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Features.Discovery;
using Hyperledger.Aries.Features.Handshakes.Connection;
using Hyperledger.Aries.Features.Handshakes.DidExchange;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Features.OutOfBand;
using Hyperledger.Aries.Features.PresentProof;
using Hyperledger.Aries.Features.RevocationNotification;
using Hyperledger.Aries.Ledger;
using Hyperledger.Aries.Ledger.V2;
using Hyperledger.Aries.Payments;
using Hyperledger.Aries.Runtime;
using Hyperledger.Aries.Signatures;
using Hyperledger.Aries.Storage;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Service builder extensions.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the agent framework required services
        /// </summary>
        /// <param name="services">Services.</param>
        public static void AddAriesFramework(this IServiceCollection services)
        {
            services.AddOptions<AgentOptions>();
            services.AddLogging();
            services.AddHttpClient();

            services.AddDefaultServices();
        }

        /// <summary>
        /// Registers the agent framework required services and invokes an agent builder
        /// </summary>
        /// <param name="services">Services.</param>
        /// <param name="builder">Builder.</param>
        public static AriesFrameworkBuilder AddAriesFramework(this IServiceCollection services, Action<AriesFrameworkBuilder> builder)
        {
            AddAriesFramework(services);

            var frameworkBuilder = new AriesFrameworkBuilder(services);
            builder.Invoke(frameworkBuilder);
            return frameworkBuilder;
        }
        
        /// <summary>
        /// Registers the agent framework required services and overwrites the default services with the next version of the services
        /// </summary>
        /// <param name="services">Services.</param>
        /// <param name="builder">Builder.</param>
        public static AriesFrameworkBuilder AddAriesFrameworkV2(this IServiceCollection services, Action<AriesFrameworkBuilder> builder)
        {
            services.AddAriesFramework();
            services.AddDefaultServicesV2();
            
            var frameworkBuilder = new AriesFrameworkBuilder(services);
            builder.Invoke(frameworkBuilder);
            return frameworkBuilder;
        }

        internal static IServiceCollection AddDefaultServices(this IServiceCollection builder)
        {
            builder.TryAddSingleton<IEventAggregator, EventAggregator>();
            builder.TryAddSingleton<IOutOfBandService, DefaultOutOfBandService>();
            builder.TryAddSingleton<IConnectionService, DefaultConnectionService>();
            builder.TryAddSingleton<ICredentialService, DefaultCredentialService>();
            builder.TryAddSingleton<IDidExchangeService, DefaultDidExchangeService>();
            builder.TryAddSingleton<ILedgerService, DefaultLedgerService>();
            builder.TryAddSingleton<ILedgerSigningService, DefaultLedgerSigningService>();
            builder.TryAddSingleton<IPoolService, DefaultPoolService>();
            builder.TryAddSingleton<IProofService, DefaultProofService>();
            builder.TryAddSingleton<IDiscoveryService, DefaultDiscoveryService>();
            builder.TryAddSingleton<IProvisioningService, DefaultProvisioningService>();
            builder.TryAddSingleton<IMessageService, DefaultMessageService>();
            builder.TryAddSingleton<IMessageDispatcher, HttpMessageDispatcher>();
            builder.TryAddSingleton<ISchemaService, DefaultSchemaService>();
            builder.TryAddSingleton<ITailsService, DefaultTailsService>();
            builder.TryAddSingleton<IWalletRecordService, DefaultWalletRecordService>();
            builder.TryAddSingleton<IWalletService, DefaultWalletService>();
            builder.TryAddSingleton<IPaymentService, DefaultPaymentService>();
            builder.TryAddSingleton<IRevocationNotificationService, DefaultRevocationNotificationService>();

            return builder;
        }
        
        internal static IServiceCollection AddDefaultServicesV2(this IServiceCollection builder)
        {
            builder.AddSingleton<ILedgerService, DefaultLedgerServiceV2>();
            builder.AddSingleton<IPoolService, DefaultPoolServiceV2>();
            builder.AddSingleton<ISigningService, DefaultSigningService>();

            return builder;
        }

        /// <summary>
        /// Adds the extended protocol discovery service.
        /// </summary>
        /// <returns>The extended ephemeral challenge service.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="TService">The 1st type parameter.</typeparam>
        /// <typeparam name="TImplementation">The 2nd type parameter.</typeparam>
        public static IServiceCollection AddExtendedDiscoveryService<TService, TImplementation>(this IServiceCollection builder)
            where TService : class, IDiscoveryService
            where TImplementation : class, TService, IDiscoveryService
        {
            builder.AddSingleton<TImplementation>();
            builder.AddSingleton<IDiscoveryService>(x => x.GetService<TImplementation>());
            builder.AddSingleton<TService>(x => x.GetService<TImplementation>());
            return builder;
        }

        /// <summary>
        /// Adds the extended connection service.
        /// </summary>
        /// <returns>The extended connection service.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="TService">The 1st type parameter.</typeparam>
        /// <typeparam name="TImplementation">The 2nd type parameter.</typeparam>
        public static IServiceCollection AddExtendedConnectionService<TService, TImplementation>(this IServiceCollection builder)
            where TService : class, IConnectionService
            where TImplementation : class, TService, IConnectionService
        {
            builder.AddSingleton<TImplementation>();
            builder.AddSingleton<IConnectionService>(x => x.GetService<TImplementation>());
            builder.AddSingleton<TService>(x => x.GetService<TImplementation>());
            return builder;
        }

        /// <summary>
        /// Adds the extended connection service.
        /// </summary>
        /// <returns>The extended connection service.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="TImplementation">The 1st type parameter.</typeparam>
        public static IServiceCollection AddExtendedConnectionService<TImplementation>(this IServiceCollection builder)
            where TImplementation : class, IConnectionService
        {
            builder.AddSingleton<IConnectionService, TImplementation>();
            return builder;
        }

        /// <summary>
        /// Adds the extended credential service.
        /// </summary>
        /// <returns>The extended credential service.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="TService">The 1st type parameter.</typeparam>
        /// <typeparam name="TImplementation">The 2nd type parameter.</typeparam>
        public static IServiceCollection AddExtendedCredentialService<TService, TImplementation>(this IServiceCollection builder)
            where TService : class, ICredentialService
            where TImplementation : class, TService, ICredentialService
        {
            builder.AddSingleton<TImplementation>();
            builder.AddSingleton<ICredentialService>(x => x.GetService<TImplementation>());
            builder.AddSingleton<TService>(x => x.GetService<TImplementation>());
            return builder;
        }

        /// <summary>
        /// Adds the extended credential service.
        /// </summary>
        /// <returns>The extended credential service.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="TImplementation">The 1st type parameter.</typeparam>
        public static IServiceCollection AddExtendedCredentialService<TImplementation>(this IServiceCollection builder)
            where TImplementation : class, ICredentialService
        {
            builder.AddSingleton<ICredentialService, TImplementation>();
            return builder;
        }

        /// <summary>
        /// Adds the extended ledger service.
        /// </summary>
        /// <returns>The extended ledger service.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="TService">The 1st type parameter.</typeparam>
        /// <typeparam name="TImplementation">The 2nd type parameter.</typeparam>
        public static IServiceCollection AddExtendedLedgerService<TService, TImplementation>(this IServiceCollection builder)
            where TService : class, ILedgerService
            where TImplementation : class, TService, ILedgerService
        {
            builder.AddSingleton<TImplementation>();
            builder.AddSingleton<ILedgerService>(x => x.GetService<TImplementation>());
            builder.AddSingleton<TService>(x => x.GetService<TImplementation>());
            return builder;
        }

        /// <summary>
        /// Adds the extended ledger service.
        /// </summary>
        /// <returns>The extended ledger service.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="TImplementation">The 1st type parameter.</typeparam>
        public static IServiceCollection AddExtendedLedgerService<TImplementation>(this IServiceCollection builder)
            where TImplementation : class, ILedgerService
        {
            builder.AddSingleton<ILedgerService, TImplementation>();
            return builder;
        }

        /// <summary>
        /// Adds the extended pool service.
        /// </summary>
        /// <returns>The extended pool service.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="TService">The 1st type parameter.</typeparam>
        /// <typeparam name="TImplementation">The 2nd type parameter.</typeparam>
        public static IServiceCollection AddExtendedPoolService<TService, TImplementation>(this IServiceCollection builder)
            where TService : class, IPoolService
            where TImplementation : class, TService, IPoolService
        {
            builder.AddSingleton<TImplementation>();
            builder.AddSingleton<IPoolService>(x => x.GetService<TImplementation>());
            builder.AddSingleton<TService>(x => x.GetService<TImplementation>());
            return builder;
        }

        /// <summary>
        /// Adds the extended pool service.
        /// </summary>
        /// <returns>The extended pool service.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="TImplementation">The 1st type parameter.</typeparam>
        public static IServiceCollection AddExtendedPoolService<TImplementation>(this IServiceCollection builder)
            where TImplementation : class, IPoolService
        {
            builder.AddSingleton<IPoolService, TImplementation>();
            return builder;
        }

        /// <summary>
        /// Adds the extended proof service.
        /// </summary>
        /// <returns>The extended proof service.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="TService">The 1st type parameter.</typeparam>
        /// <typeparam name="TImplementation">The 2nd type parameter.</typeparam>
        public static IServiceCollection AddExtendedProofService<TService, TImplementation>(this IServiceCollection builder)
            where TService : class, IProofService
            where TImplementation : class, TService, IProofService
        {
            builder.AddSingleton<TImplementation>();
            builder.AddSingleton<IProofService>(x => x.GetService<TImplementation>());
            builder.AddSingleton<TService>(x => x.GetService<TImplementation>());
            return builder;
        }

        /// <summary>
        /// Adds the extended proof service.
        /// </summary>
        /// <returns>The extended proof service.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="TImplementation">The 1st type parameter.</typeparam>
        public static IServiceCollection AddExtendedProofService<TImplementation>(this IServiceCollection builder)
            where TImplementation : class, IProofService
        {
            builder.AddSingleton<IProofService, TImplementation>();
            return builder;
        }

        /// <summary>
        /// Adds the extended provisioning service.
        /// </summary>
        /// <returns>The extended provisioning service.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="TService">The 1st type parameter.</typeparam>
        /// <typeparam name="TImplementation">The 2nd type parameter.</typeparam>
        public static IServiceCollection AddExtendedProvisioningService<TService, TImplementation>(this IServiceCollection builder)
            where TService : class, IProvisioningService
            where TImplementation : class, TService, IProvisioningService
        {
            builder.AddSingleton<TImplementation>();
            builder.AddSingleton<IProvisioningService>(x => x.GetService<TImplementation>());
            builder.AddSingleton<TService>(x => x.GetService<TImplementation>());
            return builder;
        }

        /// <summary>
        /// Adds the extended provisioning service.
        /// </summary>
        /// <returns>The extended provisioning service.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="TImplementation">The 1st type parameter.</typeparam>
        public static IServiceCollection AddExtendedProvisioningService<TImplementation>(this IServiceCollection builder)
            where TImplementation : class, IProvisioningService
        {
            builder.AddSingleton<IProvisioningService, TImplementation>();
            return builder;
        }

        /// <summary>
        /// Adds the extended message service.
        /// </summary>
        /// <returns>The extended message service.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="TService">The 1st type parameter.</typeparam>
        /// <typeparam name="TImplementation">The 2nd type parameter.</typeparam>
        public static IServiceCollection AddExtendedMessageService<TService, TImplementation>(this IServiceCollection builder)
            where TService : class, IMessageService
            where TImplementation : class, TService, IMessageService
        {
            builder.AddSingleton<TImplementation>();
            builder.AddSingleton<IMessageService>(x => x.GetService<TImplementation>());
            builder.AddSingleton<TService>(x => x.GetService<TImplementation>());
            return builder;
        }

        /// <summary>
        /// Adds the extended message service.
        /// </summary>
        /// <returns>The extended message service.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="TImplementation">The 1st type parameter.</typeparam>
        public static IServiceCollection AddExtendedMessageService<TImplementation>(this IServiceCollection builder)
            where TImplementation : class, IMessageService
        {
            builder.AddSingleton<IMessageService, TImplementation>();
            return builder;
        }

        /// <summary>
        /// Adds the extended schema service.
        /// </summary>
        /// <returns>The extended schema service.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="TService">The 1st type parameter.</typeparam>
        /// <typeparam name="TImplementation">The 2nd type parameter.</typeparam>
        public static IServiceCollection AddExtendedSchemaService<TService, TImplementation>(this IServiceCollection builder)
            where TService : class, ISchemaService
            where TImplementation : class, TService, ISchemaService
        {
            builder.AddSingleton<TImplementation>();
            builder.AddSingleton<ISchemaService>(x => x.GetService<TImplementation>());
            builder.AddSingleton<TService>(x => x.GetService<TImplementation>());
            return builder;
        }

        /// <summary>
        /// Adds the extended schema service.
        /// </summary>
        /// <returns>The extended schema service.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="TImplementation">The 1st type parameter.</typeparam>
        public static IServiceCollection AddExtendedSchemaService<TImplementation>(this IServiceCollection builder)
            where TImplementation : class, ISchemaService
        {
            builder.AddSingleton<ISchemaService, TImplementation>();
            return builder;
        }

        /// <summary>
        /// Adds the extended tails service.
        /// </summary>
        /// <returns>The extended tails service.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="TService">The 1st type parameter.</typeparam>
        /// <typeparam name="TImplementation">The 2nd type parameter.</typeparam>
        public static IServiceCollection AddExtendedTailsService<TService, TImplementation>(this IServiceCollection builder)
            where TService : class, ITailsService
            where TImplementation : class, TService, ITailsService
        {
            builder.AddSingleton<TImplementation>();
            builder.AddSingleton<ITailsService>(x => x.GetService<TImplementation>());
            builder.AddSingleton<TService>(x => x.GetService<TImplementation>());
            return builder;
        }

        /// <summary>
        /// Adds the extended tails service.
        /// </summary>
        /// <returns>The extended tails service.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="TImplementation">The 1st type parameter.</typeparam>
        public static IServiceCollection AddExtendedTailsService<TImplementation>(this IServiceCollection builder)
            where TImplementation : class, ITailsService
        {
            builder.AddSingleton<ITailsService, TImplementation>();
            return builder;
        }

        /// <summary>
        /// Adds the extended wallet record service.
        /// </summary>
        /// <returns>The extended wallet record service.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="TService">The 1st type parameter.</typeparam>
        /// <typeparam name="TImplementation">The 2nd type parameter.</typeparam>
        public static IServiceCollection AddExtendedWalletRecordService<TService, TImplementation>(this IServiceCollection builder)
            where TService : class, IWalletRecordService
            where TImplementation : class, TService, IWalletRecordService
        {
            builder.AddSingleton<TImplementation>();
            builder.AddSingleton<IWalletRecordService>(x => x.GetService<TImplementation>());
            builder.AddSingleton<TService>(x => x.GetService<TImplementation>());
            return builder;
        }

        /// <summary>
        /// Adds the extended wallet record service.
        /// </summary>
        /// <returns>The extended wallet record service.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="TImplementation">The 1st type parameter.</typeparam>
        public static IServiceCollection AddExtendedWalletRecordService<TImplementation>(this IServiceCollection builder)
            where TImplementation : class, IWalletRecordService
        {
            builder.AddSingleton<IWalletRecordService, TImplementation>();
            return builder;
        }

        /// <summary>
        /// Adds the extended wallet service.
        /// </summary>
        /// <returns>The extended wallet service.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="TService">The 1st type parameter.</typeparam>
        /// <typeparam name="TImplementation">The 2nd type parameter.</typeparam>
        public static IServiceCollection AddExtendedWalletService<TService, TImplementation>(this IServiceCollection builder)
            where TService : class, IWalletService
            where TImplementation : class, TService, IWalletService
        {
            builder.AddSingleton<TImplementation>();
            builder.AddSingleton<IWalletService>(x => x.GetService<TImplementation>());
            builder.AddSingleton<TService>(x => x.GetService<TImplementation>());
            return builder;
        }

        /// <summary>
        /// Adds the extended wallet service.
        /// </summary>
        /// <returns>The extended wallet service.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="TImplementation">The 1st type parameter.</typeparam>
        public static IServiceCollection AddExtendedWalletService<TImplementation>(this IServiceCollection builder)
            where TImplementation : class, IWalletService
        {
            builder.AddSingleton<IWalletService, TImplementation>();
            return builder;
        }
    }
}
