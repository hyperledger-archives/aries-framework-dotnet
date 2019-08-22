using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Messages;
using AgentFramework.Core.Messages.EphemeralChallenge;

namespace AgentFramework.Core.Handlers.Internal
{
    internal class DefaultEphemeralChallengeHandler : MessageHandlerBase<EphemeralChallengeResponseMessage>
    {
        private readonly IEphemeralChallengeService _ephemeralChallengeService;

        /// <summary>Initializes a new instance of the <see cref="DefaultEphemeralChallengeHandler"/> class.</summary>
        /// <param name="ephemeralChallengeService">The ephemeral challenge service.</param>
        public DefaultEphemeralChallengeHandler(IEphemeralChallengeService ephemeralChallengeService)
        {
            _ephemeralChallengeService = ephemeralChallengeService;
        }

        protected override async Task<AgentMessage> ProcessAsync(EphemeralChallengeResponseMessage message, IAgentContext agentContext, MessageContext messageContext)
        {
            await _ephemeralChallengeService.ProcessChallengeResponseAsync(agentContext, message);
            return null;
        }
    }
}