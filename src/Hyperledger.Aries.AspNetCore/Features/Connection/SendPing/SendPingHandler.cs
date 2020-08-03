namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using Hyperledger.Aries.Agents;
  using Hyperledger.Aries.Configuration;
  using Hyperledger.Aries.Contracts;
  using Hyperledger.Aries.Features.DidExchange;
  using Hyperledger.Aries.Features.TrustPing;
  using Hyperledger.Aries.Models.Events;
  using Hyperledger.Aries.Storage;
  using MediatR;
  using Microsoft.Extensions.Options;
  using System;
  using System.Collections.Generic;
  using System.Reactive.Subjects;
  using System.Linq;
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

    private readonly IAgentProvider AgentProvider;
    private readonly IEventAggregator EventAggregator;
    private readonly IWalletService WalletService;
    private readonly IConnectionService ConnectionService;
    private readonly IWalletRecordService WalletRecordService;
    private readonly IMessageService MessageService;
    private readonly AgentOptions AgentOptions;

    public SendPingHandler
    (
      IAgentProvider aAgentProvider,
      IEventAggregator aEventAggregator,
      IWalletService aWalletService,
      IOptions<AgentOptions> aAgentOptions,
      IConnectionService aConnectionService,
      IWalletRecordService aWalletRecordService,
      IMessageService aMessageService
    )
    {
      AgentProvider = aAgentProvider;
      EventAggregator = aEventAggregator;
      WalletService = aWalletService;
      ConnectionService = aConnectionService;
      WalletRecordService = aWalletRecordService;
      MessageService = aMessageService;
      AgentOptions = aAgentOptions.Value;
    }


    public async Task<SendPingResponse> Handle
    (
      SendPingRequest aSendPingRequest,
      CancellationToken aCancellationToken
    )
    {
      IAgentContext agentContext = await AgentProvider.GetContextAsync();

      ConnectionRecord connectionRecord =
        await ConnectionService.GetAsync(agentContext, aSendPingRequest.ConnectionId);

      var trustPingMessage = new TrustPingMessage
      {
        ResponseRequested = true,
        Comment = "Hello"
      };

      var semaphoreSlim = new SemaphoreSlim(0, 1);

      bool success = false;

      using
      (
        IDisposable subscription = EventAggregator.GetEventByType<ServiceMessageProcessingEvent>()
        .Where
        (
          aServiceMessageProcessingEvent =>
            aServiceMessageProcessingEvent.MessageType == TrustPingResponseMessageType
        )
        .Subscribe(_ => { success = true; semaphoreSlim.Release(); })
      )
      {
        await MessageService.SendAsync(agentContext.Wallet, trustPingMessage, connectionRecord);

        await semaphoreSlim.WaitAsync(TimeSpan.FromSeconds(5));
      }

      var response = new SendPingResponse(aSendPingRequest.CorrelationId, success);

      return response;
    }
  }
}
