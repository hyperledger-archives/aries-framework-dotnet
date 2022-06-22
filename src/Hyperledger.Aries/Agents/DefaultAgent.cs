using System;

namespace Hyperledger.Aries.Agents
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
            AddOutOfBandHandler();
            AddConnectionHandler();
            AddCredentialHandler();
            AddDidExchangeHandler();
            AddProofHandler();
            AddDiscoveryHandler();
            AddBasicMessageHandler();
            AddForwardHandler();
            AddTrustPingHandler();
            AddRevocationNotificationHandler();
        }
    }
}
