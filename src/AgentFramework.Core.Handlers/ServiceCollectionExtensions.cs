using AgentFramework.Core.Handlers;
using AgentFramework.Core.Handlers.Agents;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Service collection extensions
    /// </summary>
    public static class ServiceCollectionExtensions
    {
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
