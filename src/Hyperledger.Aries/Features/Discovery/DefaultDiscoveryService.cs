using System;
using System.Collections.Generic;
using System.Linq;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Decorators.Threading;
using Hyperledger.Aries.Extensions;
using Microsoft.Extensions.Logging;

namespace Hyperledger.Aries.Features.Discovery
{
    /// <inheritdoc />
    public class DefaultDiscoveryService : IDiscoveryService
    {
        /// <summary>
        /// The event aggregator.
        /// </summary>
        protected readonly IEventAggregator EventAggregator;
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger<DefaultDiscoveryService> Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDiscoveryService"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="logger">The logger.</param>
        public DefaultDiscoveryService(
            IEventAggregator eventAggregator,
            ILogger<DefaultDiscoveryService> logger)
        {
            EventAggregator = eventAggregator;
            Logger = logger;
        }

        /// <inheritdoc />
        public virtual DiscoveryQueryMessage CreateQuery(IAgentContext agentContext, string query)
        {
            if (string.IsNullOrEmpty(query))
                throw new ArgumentNullException(nameof(query));

            //TODO validate this is a valid query????

            return new DiscoveryQueryMessage(agentContext.UseMessageTypesHttps)
            {
                Query = query
            };
        }

        /// <inheritdoc />
        public virtual DiscoveryDiscloseMessage CreateQueryResponse(IAgentContext agentContext, DiscoveryQueryMessage message)
        {
            if (string.IsNullOrEmpty(message.Query))
                throw new ArgumentNullException(nameof(message.Query));

            //TODO validate this is a valid query????

            var test = message.Query.TrimEnd('*');

            IList<MessageType> supportedMessages = new List<MessageType>();
            if (message.Query == "*")
                supportedMessages = agentContext.SupportedMessages;
            else if (message.Query.EndsWith("*"))
                supportedMessages = agentContext.SupportedMessages?.Where(_ => _.MessageTypeUri.StartsWith(message.Query.TrimEnd('*'))).ToList();
            else
                supportedMessages = agentContext.SupportedMessages?.Where(_ => _.MessageTypeUri == message.Query).ToList();

            supportedMessages ??= new List<MessageType>();

            var disclosureMessage = message.CreateThreadedReply<DiscoveryDiscloseMessage>();
            foreach (var supportedMessage in supportedMessages.GroupBy(_ => _.MessageFamilyUri).Select(g => g.First()))
            {
                disclosureMessage.Protocols.Add(new DisclosedMessageProtocol
                {
                    ProtocolId = supportedMessage.MessageFamilyUri
                });
            }

            return disclosureMessage;
        }
    }
}
