using System.Linq;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Decorators.Payments;
using AgentFramework.Core.Handlers.Agents;
using AgentFramework.Core.Messages;
using AgentFramework.Core.Models.Records;
using AgentFramework.Core.Models.Records.Search;

namespace AgentFramework.Payments.SovrinToken
{
    public class PaymentsAgentMiddleware : IAgentMiddleware
    {
        private readonly IPaymentService _paymentService;
        private readonly IWalletRecordService _recordService;

        public PaymentsAgentMiddleware(
            IPaymentService paymentService,
            IWalletRecordService recordService)
        {
            _paymentService = paymentService;
            _recordService = recordService;
        }

        public async Task OnMessageAsync(IAgentContext agentContext, MessageContext messageContext)
        {
            var message = messageContext.GetMessage<InternalAgentMessage>();
            var requestDecorator = message.FindDecorator<PaymentRequestDecorator>("payment_request");
            if (requestDecorator != null)
            {
                var record = new PaymentRecord
                {
                    ConnectionId = messageContext.Connection.Id,
                    Details = requestDecorator.Details,
                    ReferenceId = requestDecorator.Details.Id,
                    Address = requestDecorator.Method.Data.PayeeId,
                    Amount = requestDecorator.Details.Total.Amount.Value
                };
                await record.TriggerAsync(PaymentTrigger.RequestReceived);
                await _recordService.AddAsync(agentContext.Wallet, record);

                if (messageContext.ContextRecord != null)
                {
                    messageContext.ContextRecord.SetTag("PaymentRecordId", record.Id);
                    await _recordService.UpdateAsync(agentContext.Wallet, messageContext.ContextRecord);
                }
            }

            var receiptDecorator = message.FindDecorator<PaymentReceiptDecorator>("payment_receipt");
            if (receiptDecorator != null)
            {
                var search = await _recordService.SearchAsync<PaymentRecord>(
                    wallet: agentContext.Wallet,
                    query: SearchQuery.Equal(nameof(PaymentRecord.ReferenceId), receiptDecorator.RequestId),
                    options: null,
                    count: 5);
                var record = search.FirstOrDefault() ?? new PaymentRecord();
                record.ReceiptId = receiptDecorator.TransactionId;

                await record.TriggerAsync(PaymentTrigger.ReceiptReceived);

                if (search.Any())
                {
                    await _recordService.UpdateAsync(agentContext.Wallet, record);
                }
                else
                {
                    await _recordService.AddAsync(agentContext.Wallet, record);
                }
            }
        }
    }
}
