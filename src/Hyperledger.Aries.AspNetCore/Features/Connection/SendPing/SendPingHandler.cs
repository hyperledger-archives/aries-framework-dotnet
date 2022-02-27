namespace Hyperledger.Aries.AspNetCore.Features.Connection.SendPing
{
  using Agents;
  using Aries.Features.DidExchange;
  using Aries.Features.TrustPing;
  using Models.Events;
  using Connections;
  using Contracts;
  using MediatR;
  using System;
  using System.Reactive.Linq;
  using System.Threading;
  using System.Threading.Tasks;

  public class SendPingHandler : IRequestHandler<SendPingRequest, SendPingResponse>
  {
    /// <summary>
    /// Ping Response Message Type.
    /// </summary>
    public const string TrustPingResponseMessageType = 
      "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/trust_ping/1.0/ping_response";

    private readonly IAgentProvider _agentProvider;
    private readonly IEventAggregator _eventAggregator;
    private readonly IConnectionService _connectionService;
    private readonly IMessageService _messageService;

    public SendPingHandler
    (
      IAgentProvider aAgentProvider,
      IEventAggregator aEventAggregator,
      IConnectionService aConnectionService,
      IMessageService aMessageService
    )
    {
      _agentProvider = aAgentProvider;
      _eventAggregator = aEventAggregator;
      _connectionService = aConnectionService;
      _messageService = aMessageService;
    }


    public async Task<SendPingResponse> Handle
    (
      SendPingRequest aSendPingRequest,
      CancellationToken aCancellationToken
    )
    {
      IAgentContext agentContext = await _agentProvider.GetContextAsync();

      ConnectionRecord connectionRecord =
        await _connectionService.GetAsync(agentContext, aSendPingRequest.ConnectionId);

      var trustPingMessage = new TrustPingMessage(agentContext.UseMessageTypesHttps)
      {
        ResponseRequested = true,
        Comment = "Hello"
      };

      var semaphoreSlim = new SemaphoreSlim(0, 1);

      bool success = false;

      using
      (
        _eventAggregator.GetEventByType<ServiceMessageProcessingEvent>()
        .Where
        (
          aServiceMessageProcessingEvent =>
            aServiceMessageProcessingEvent.MessageType == TrustPingResponseMessageType
        )
        .Subscribe(_ => { success = true; semaphoreSlim.Release(); })
      )
      {
        await _messageService.SendAsync(agentContext, trustPingMessage, connectionRecord);
        await semaphoreSlim.WaitAsync(TimeSpan.FromSeconds(5), aCancellationToken);
      }

      var response = new SendPingResponse(aSendPingRequest.CorrelationId, success);
      return response;
    }
  }
}