using System;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Agents.Edge;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Routing;
using Hyperledger.Aries.Routing.Edge;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AriesFrameworkBuilderExtensions
    {
        /// <summary>
        /// Registers and provisions an agent.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        /// <param name="delayProvisioning"></param>
        /// <returns></returns>
        public static AriesFrameworkBuilder RegisterEdgeAgent
        (
            this AriesFrameworkBuilder builder,
            Action<AgentOptions> options,
            bool delayProvisioning = false)
            => RegisterEdgeAgent<DefaultAgent>(builder, options, delayProvisioning
        );

        /// <summary>
        /// Registers and provisions an agent with custom implementation
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        /// <param name="delayProvisioning"></param>
        /// <returns></returns>
        public static AriesFrameworkBuilder RegisterEdgeAgent<T>
        (
            this AriesFrameworkBuilder builder,
            Action<AgentOptions> options,
            bool delayProvisioning = false
        ) where T : class, IAgent
        {
            builder.AddAgentProvider();
            builder.Services.AddDefaultMessageHandlers();
            builder.Services.AddSingleton<IAgent, T>();
            builder.Services.Configure(options);
            builder.Services.AddSingleton<IEdgeClientService, EdgeClientService>();
            builder.Services.AddSingleton<IEdgeProvisioningService, EdgeProvisioningService>();
            builder.Services.AddExtendedConnectionService<EdgeConnectionService>();
            if (!delayProvisioning)
            {
                builder.Services.AddHostedService<EdgeProvisioningService>();
            }

            return builder;
        }
    }
}
