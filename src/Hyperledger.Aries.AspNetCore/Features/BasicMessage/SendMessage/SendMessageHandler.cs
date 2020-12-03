namespace Hyperledger.Aries.AspNetCore.Features.BasicMessaging
{
  using Hyperledger.Aries.Agents;
  using Hyperledger.Aries.Configuration;
  using Hyperledger.Aries.Features.BasicMessage;
  using Hyperledger.Aries.Features.DidExchange;
  using Hyperledger.Aries.Storage;
  using MediatR;
  using Microsoft.Extensions.Options;
  using System;
  using System.Globalization;
  using System.Threading;
  using System.Threading.Tasks;

  public class SendMessageHandler : IRequestHandler<SendMessageRequest, SendMessageResponse>
  {
    private readonly AgentOptions AgentOptions;
    private readonly IConnectionService ConnectionService;
    private readonly IMessageService MessageService;
    private readonly IWalletRecordService WalletRecordService;
    private readonly IWalletService WalletService;

    public SendMessageHandler
    (
      IWalletService aWalletService,
      IOptions<AgentOptions> aAgentOptions,
      IConnectionService aConnectionService,
      IWalletRecordService aWalletRecordService,
      IMessageService aMessageService
    )
    {
      WalletService = aWalletService;
      ConnectionService = aConnectionService;
      WalletRecordService = aWalletRecordService;
      MessageService = aMessageService;
      AgentOptions = aAgentOptions.Value;
    }

    public async Task<SendMessageResponse> Handle
    (
      SendMessageRequest aSendMessageRequest,
      CancellationToken aCancellationToken
    )
    {
      var defaultAgentContext = new DefaultAgentContext
      {
        Wallet = await WalletService.GetWalletAsync(AgentOptions.WalletConfiguration, AgentOptions.WalletCredentials)
      };

      DateTime sentTime = DateTime.UtcNow;

      var messageRecord = new BasicMessageRecord
      {
        Id = Guid.NewGuid().ToString(),
        Direction = MessageDirection.Outgoing,
        Text = aSendMessageRequest.Message,
        SentTime = sentTime,
        ConnectionId = aSendMessageRequest.ConnectionId
      };

      var basicMessage = new BasicMessage
      {
        Content = aSendMessageRequest.Message,
        SentTime = sentTime.ToString("s", CultureInfo.InvariantCulture),
        UseMessageTypesHttps = defaultAgentContext.UseMessageTypesHttps
      };

      ConnectionRecord connectionRecord =
        await ConnectionService.GetAsync(defaultAgentContext, aSendMessageRequest.ConnectionId);

      // Save the outgoing message to the local wallet for chat history purposes
      await WalletRecordService.AddAsync(defaultAgentContext.Wallet, messageRecord);

      // Send an agent message using the secure connection
      await MessageService.SendAsync(defaultAgentContext, basicMessage, connectionRecord);

      var response = new SendMessageResponse(aSendMessageRequest.CorrelationId);

      return response;
    }
  }
}
