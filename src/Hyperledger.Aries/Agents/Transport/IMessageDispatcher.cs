using System;
using System.Threading.Tasks;

namespace Hyperledger.Aries.Agents
{
    /// <summary>
    /// Message Dispatcher.
    /// </summary>
    public interface IMessageDispatcher
    {
        /// <summary>
        /// Supported transport schemes by the dispatcher.
        /// </summary>
        string[] TransportSchemes { get; }

        /// <summary>
        /// Sends a message using the dispatcher.
        /// </summary>
        /// <param name="uri">Uri to dispatch the message to.</param>
        /// <param name="message">Message context to dispatch.</param>
        /// <returns>A message context.</returns>
        Task<PackedMessageContext> DispatchAsync(Uri uri, PackedMessageContext message);
    }
}
