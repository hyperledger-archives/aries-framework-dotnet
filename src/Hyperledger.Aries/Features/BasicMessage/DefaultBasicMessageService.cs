using System;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Features.BasicMessage;

/// <inheritdoc />
public class DefaultBasicMessageService : IBasicMessageService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultBasicMessageService"/> class.
    /// </summary>
    /// <param name="recordService">The record service.</param>
    public DefaultBasicMessageService(IWalletRecordService recordService)
    {
        RecordService = recordService;
    }
    
    protected readonly IWalletRecordService RecordService;

    /// <inheritdoc />
    public virtual async Task<BasicMessageRecord> ProcessIncomingBasicMessageAsync(IAgentContext agentContext, UnpackedMessageContext unpackedMessageContext,
        BasicMessage basicMessage)
    {
        var record = new BasicMessageRecord
        {
            Id = Guid.NewGuid().ToString(),
            ConnectionId = unpackedMessageContext.Connection.Id,
            Text = basicMessage.Content,
            SentTime = DateTime.TryParse(basicMessage.SentTime, out var dateTime) ? dateTime : DateTime.UtcNow,
            Direction = MessageDirection.Incoming
        };
        await RecordService.AddAsync(agentContext.Wallet, record);
        
        return record;
    }
}
