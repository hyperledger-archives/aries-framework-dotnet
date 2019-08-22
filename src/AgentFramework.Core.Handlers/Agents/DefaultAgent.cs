using System;

namespace AgentFramework.Core.Handlers.Agents
{
    /// <summary>
    /// Default agent.
    /// </summary>
    public class DefaultAgent : AgentBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AgentFramework.Core.Handlers.Agents.DefaultAgent"/> class.
        /// </summary>
        /// <param name="provider">Provider.</param>
        public DefaultAgent(IServiceProvider provider) : base(provider)
        {
        }

        /// <summary>
        /// Configures the handlers.
        /// </summary>
        protected override void ConfigureHandlers()
        {
            AddConnectionHandler();
            AddForwardHandler();
        }
    }
}
