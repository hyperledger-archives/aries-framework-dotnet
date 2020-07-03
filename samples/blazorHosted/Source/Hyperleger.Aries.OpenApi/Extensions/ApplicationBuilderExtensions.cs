//using Hyperledger.Aries.Agents;
//using Hyperledger.Aries.AspNetCore;
//using Hyperledger.Aries.Configuration;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.Extensions.Options;

//namespace Microsoft.Extensions.DependencyInjection
//{
//    /// <summary>
//    /// <see cref="IServiceCollection"/> extension methods
//    /// </summary>
//    public static class ApplicationBuilderExtensions
//    {
//        /// <summary>
//        /// Allows default agent configuration
//        /// </summary>
//        /// <param name="app">App.</param>
//        public static void UseAriesFramework(this IApplicationBuilder app) => UseAriesFramework<AgentMiddleware>(app);

//        /// <summary>
//        /// Allows agent configuration by specifying a custom middleware
//        /// </summary>
//        /// <param name="app">App.</param>
//        public static void UseAriesFramework<T>(this IApplicationBuilder app)
//        {
//            var options = app.ApplicationServices.GetRequiredService<IOptions<AgentOptions>>().Value;

//            app.UseMiddleware<T>();
//            app.MapWhen(
//                context => context.Request.Path.ToUriComponent().Contains(options.RevocationRegistryUriPath), 
//                app => app.UseMiddleware<TailsMiddleware>());
//        }
//    }
//}