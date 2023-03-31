using System.Threading.Tasks;
using Hyperledger.Aries.Agents;

namespace Hyperledger.Aries.Features.BasicMessage;

/// <summary>
///  Basic message service
/// </summary>
public interface IBasicMessageService
{
    /// <summary>
    /// Processes the incoming basic message.
    /// </summary>
    /// <param name="agentContext">The agent context.</param>
    /// <param name="unpackedMessageContext">The unpacked message context.</param>
    /// <param name="basicMessage">The basic message.</param>
    /// <returns>Records associated with the message.</returns>
    Task<BasicMessageRecord> ProcessIncomingBasicMessageAsync(IAgentContext agentContext, UnpackedMessageContext unpackedMessageContext, BasicMessage basicMessage);
}
