using System;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Decorators.Threading;
using Hyperledger.Aries.Models.Events;
using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Features.BasicMessage;

/// <inheritdoc />
public class DefaultBasicMessageService : IBasicMessageService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultBasicMessageService"/> class.
    /// </summary>
    /// <param name="eventAggregator">The event aggregator.</param>
    /// <param name="recordService">The record service.</param>
    public DefaultBasicMessageService(
        IEventAggregator eventAggregator,
        IWalletRecordService recordService)
    {
        EventAggregator = eventAggregator;
        RecordService = recordService;
    }

    protected readonly IEventAggregator EventAggregator;
    protected readonly IWalletRecordService RecordService;

    /// <inheritdoc />
    public virtual async Task<BasicMessageRecord> ProcessIncomingBasicMessageAsync(IAgentContext agentContext, string connectionId,
        BasicMessage basicMessage)
    {
        var record = new BasicMessageRecord
        {
            Id = Guid.NewGuid().ToString(),
            ConnectionId = connectionId,
            Text = basicMessage.Content,
            SentTime = DateTime.TryParse(basicMessage.SentTime, out var dateTime) ? dateTime : DateTime.UtcNow,
            Direction = MessageDirection.Incoming
        };
        await RecordService.AddAsync(agentContext.Wallet, record);
        
        EventAggregator.Publish(new ServiceMessageProcessingEvent
        {
            MessageType = basicMessage.Type,
            RecordId = record.Id,
            ThreadId = basicMessage.GetThreadId()
        });

        return record;
    }
}
