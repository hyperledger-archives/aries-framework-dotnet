﻿using System;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Agents.Edge;
using Hyperledger.Aries.AspNetCore;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AriesFrameworkBuilderExtensions
    {
        /// <summary>
        /// Registers and provisions an agent.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static AriesFrameworkBuilder RegisterMediatorAgent(
            this AriesFrameworkBuilder builder,
            Action<AgentOptions> options)
            => RegisterMediatorAgent<DefaultMediatorAgent>(builder, options);

        /// <summary>
        /// Registers and provisions an agent with custom implementation
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static AriesFrameworkBuilder RegisterMediatorAgent<T>(
            this AriesFrameworkBuilder builder,
            Action<AgentOptions> options)
            where T : class, IAgent
        {
            builder.AddAgentProvider();
            builder.Services.AddDefaultMessageHandlers();
            builder.Services.AddMessageHandler<MediatorForwardHandler>();
            builder.Services.AddMessageHandler<RoutingInboxHandler>();
            builder.Services.AddSingleton<IRoutingStore, DefaultRoutingStore>();
            builder.Services.AddSingleton<IAgent, T>();
            builder.Services.Configure(options);
            builder.Services.AddHostedService<MediatorProvisioningService>();
            builder.Services.AddSingleton<MediatorDiscoveryMiddleware>();

            return builder;
        }

        public static void UseMediatorDiscovery(this IApplicationBuilder applicationBuilder) => applicationBuilder.MapWhen(
                context => context.Request.Path.ToString().Contains(".well-known/agent-configuration"),
                builder => builder.UseMiddleware<MediatorDiscoveryMiddleware>());
    }
}
