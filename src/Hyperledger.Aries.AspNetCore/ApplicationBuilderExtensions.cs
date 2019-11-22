using Hyperledger.Aries.Agents;
using Hyperledger.Aries.AspNetCore;
using Microsoft.AspNetCore.Builder;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// <see cref="IServiceCollection"/> extension methods
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Allows default agent configuration
        /// </summary>
        /// <param name="app">App.</param>
        public static void UseAriesFramework(this IApplicationBuilder app) => app.UseMiddleware<AgentMiddleware>();

        /// <summary>
        /// Allows agent configuration by specifying a custom middleware
        /// </summary>
        /// <param name="app">App.</param>
        public static void UseAgentFramework<T>(this IApplicationBuilder app) => app.UseMiddleware<T>();
    }
}