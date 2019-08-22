using System.Threading.Tasks;
using AgentFramework.Core.Messages.Common;
using AgentFramework.TestHarness;
using AgentFramework.TestHarness.Mock;
using Xunit;
using System.Linq;
using AgentFramework.Core.Models.Records;
using AgentFramework.Core.Models.Payments;
using AgentFramework.Core.Decorators.Payments;
using System.Collections.Generic;
using AgentFramework.Core.Models.Records.Search;

namespace AgentFramework.Core.Tests.Payments
{
    public class ProtocolTests : TestSingleWallet
    {
        [Fact(DisplayName = "Payment full integration test - request, pay, receipt, verify")]
        public async Task SendPaymentRequest()
        {
            // Create two agent hosts and establish pairwise connection between them
            var agents = await InProcAgent.CreatePairedAsync(true);

            // Get each agent payment address records
            var paymentAddress1 = await agents.Agent1.Payments.GetDefaultPaymentAddressAsync(agents.Agent1.Context);
            var paymentAddress2 = await agents.Agent2.Payments.GetDefaultPaymentAddressAsync(agents.Agent2.Context);

            // Internal reference number for this payment
            const string paymentReference = "INVOICE# 0001";

            // Setup a basic use case for payments by using basic messages
            // Any AgentMessage can be used to attach payment requests and receipts
            var basicMessage = new BasicMessage { Content = "This is payment request" };

            // Attach the payment request to the agent message
            var paymentRecord1 = await agents.Agent1.Payments.AttachPaymentRequestAsync(
                context: agents.Agent1.Context,
                agentMessage: basicMessage,
                details: new PaymentDetails
                {
                    Id = paymentReference,
                    Items = new List<PaymentItem>
                    {
                        new PaymentItem { Label = "Item 1", Amount = 8 },
                        new PaymentItem { Label = "Tax", Amount = 2 }
                    },
                    Total = new PaymentItem
                    {
                        Amount = new PaymentAmount
                        {
                            Currency = "sov",
                            Value = 10
                        },
                        Label = "Total"
                    }
                },
                payeeAddress: paymentAddress1);

            // PaymentRecord expectations
            Assert.NotNull(paymentRecord1);
            Assert.Equal(10UL, paymentRecord1.Amount);
            Assert.Equal(paymentAddress1.Address, paymentRecord1.Address);
            Assert.Equal(PaymentState.Requested, paymentRecord1.State);

            // Ensure the payment request is attached to the message
            var decorator = basicMessage.FindDecorator<PaymentRequestDecorator>("payment_request");

            Assert.NotNull(decorator);

            // Send the message to agent 2
            _ = await agents.Agent1.Messages.SendAsync(agents.Agent1.Context.Wallet, basicMessage, agents.Connection1);

            // Find the payment record in the context of agent 2
            var search = await agents.Agent2.Records.SearchAsync<PaymentRecord>(
                wallet: agents.Agent2.Context.Wallet,
                query: SearchQuery.And(
                    SearchQuery.Equal(nameof(PaymentRecord.ReferenceId), paymentReference),
                    SearchQuery.Equal(nameof(PaymentRecord.ConnectionId), agents.Connection2.Id)));
            var paymentRecord2 = search.FirstOrDefault();

            Assert.Single(search);
            Assert.NotNull(paymentRecord2);
            Assert.Equal(PaymentState.RequestReceived, paymentRecord2.State);

            // Fund payment address of agent 2 so we can make payment to agent 1
            await FundAccountAsync(50UL, paymentAddress2.Address);

            // Refresh balance to ensure it is funded correctly
            await agents.Agent2.Payments.RefreshBalanceAsync(agents.Agent2.Context, paymentAddress2);

            Assert.Equal(50UL, paymentAddress2.Balance);

            // Make payment for received request
            await agents.Agent2.Payments.MakePaymentAsync(agents.Agent2.Context, paymentRecord2, paymentAddress2);

            Assert.Equal(PaymentState.Paid, paymentRecord2.State);
            Assert.Equal(40UL, paymentAddress2.Balance);
            Assert.NotNull(paymentRecord2.ReceiptId);

            // Send a basic message back to agent 1 and attach a payment receipt
            var message2 = new BasicMessage { Content = "Here's a receipt" };

            // Attach a payment receipt to the basic message
            // Receipts can be attached to any agent message
            agents.Agent2.Payments.AttachPaymentReceipt(agents.Agent2.Context, message2, paymentRecord2);

            // Send the message to agent 1
            await agents.Agent2.Messages.SendAsync(agents.Agent2.Context.Wallet, message2, agents.Connection2);

            // Fetch payment record 1 again, to refresh state
            paymentRecord1 = await agents.Agent1.Records.GetAsync<PaymentRecord>(agents.Agent1.Context.Wallet, paymentRecord1.Id);

            // Check payment record 1 for receipt

            Assert.Equal(PaymentState.ReceiptReceived, paymentRecord1.State);
            Assert.NotNull(paymentRecord1.ReceiptId);

            // Check agent 1 balance
            await agents.Agent1.Payments.RefreshBalanceAsync(agents.Agent1.Context, paymentAddress1);

            Assert.Equal(10UL, paymentAddress1.Balance);

            // Verify the payment receipt on the ledger
            var verified = await agents.Agent1.Payments.VerifyPaymentAsync(agents.Agent1.Context, paymentRecord1);

            Assert.True(verified);

            // Clean things up and shut down hosts gracefully
            await agents.Agent1.DisposeAsync();
            await agents.Agent2.DisposeAsync();
        }
    }
}
