using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Features.BasicMessage;
using Hyperledger.Aries.Features.Discovery;
using Hyperledger.Aries.Features.Handshakes.Connection;
using Hyperledger.Aries.Features.Handshakes.DidExchange;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Features.OutOfBand;
using Hyperledger.Aries.Features.PresentProof;
using Hyperledger.Aries.Features.RevocationNotification;
using Hyperledger.Aries.Features.Routing;
using Hyperledger.Aries.Features.TrustPing;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Service collection extensions
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>Adds the default message handlers.</summary>
        /// <param name="collection">The collection.</param>
        public static void AddDefaultMessageHandlers(this IServiceCollection collection)
        {
            collection.AddTransient<DefaultConnectionHandler>();
            collection.AddTransient<DefaultCredentialHandler>();
            collection.AddTransient<DefaultDidExchangeHandler>();
            collection.AddTransient<DefaultProofHandler>();
            collection.AddTransient<DefaultForwardHandler>();
            collection.AddTransient<DefaultTrustPingMessageHandler>();
            collection.AddTransient<DefaultDiscoveryHandler>();
            collection.AddTransient<DefaultBasicMessageHandler>();
            collection.AddTransient<DefaultOutOfBandHandler>();
            collection.AddTransient<DefaultRevocationNotificationHandler>();
        }

        /// <summary>
        /// Adds the message handler.
        /// </summary>
        /// <returns>The message handler.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="TMessageHandler">The 1st type parameter.</typeparam>
        public static IServiceCollection AddMessageHandler<TMessageHandler>(this IServiceCollection builder) where TMessageHandler : class,
            IMessageHandler
        {
            builder.AddSingleton<IMessageHandler, TMessageHandler>();
            builder.TryAddSingleton<TMessageHandler>();
            return builder;
        }

        /// <summary>
        /// Overrides the default agent provider.
        /// </summary>
        /// <returns>The default agent provider.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="TProvider">The 1st type parameter.</typeparam>
        public static IServiceCollection OverrideDefaultAgentProvider<TProvider>(
            this IServiceCollection builder)
            where TProvider : class, IAgentProvider
        {
            builder.AddSingleton<IAgentProvider, TProvider>();
            return builder;
        }
    }
}
