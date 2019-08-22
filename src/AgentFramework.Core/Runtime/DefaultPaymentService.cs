using System;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Messages;
using AgentFramework.Core.Models.Payments;
using AgentFramework.Core.Models.Records;

namespace AgentFramework.Core.Runtime
{
    /// <inheritdoc />
    public class DefaultPaymentService : IPaymentService
    {
        /// <inheritdoc />
        public void AttachPaymentReceipt(IAgentContext context, AgentMessage agentMessage, PaymentRecord paymentRecord)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<PaymentRecord> AttachPaymentRequestAsync(IAgentContext context, AgentMessage agentMessage, PaymentDetails details, PaymentAddressRecord addressRecord = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<PaymentAddressRecord> CreatePaymentAddressAsync(IAgentContext agentContext, AddressOptions configuration = null)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task<TransactionCost> GetTransactionCostAsync(IAgentContext context, string transactionType, PaymentAddressRecord addressRecord = null)
        {
            return Task.FromResult<TransactionCost>(null);
        }

        /// <inheritdoc />
        public Task RefreshBalanceAsync(IAgentContext agentContext, PaymentAddressRecord paymentAddress = null)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task<PaymentAddressRecord> GetDefaultPaymentAddressAsync(IAgentContext agentContext)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<ulong> GetTransactionFeeAsync(IAgentContext agentContext, string transactionType)
        {
            return Task.FromResult(0UL);
        }

        /// <inheritdoc />
        public Task MakePaymentAsync(IAgentContext agentContext, PaymentRecord paymentRecord, PaymentAddressRecord addressRecord = null)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task SetDefaultPaymentAddressAsync(IAgentContext agentContext, PaymentAddressRecord addressRecord)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task<bool> VerifyPaymentAsync(IAgentContext context, PaymentRecord paymentRecord)
        {
            throw new NotImplementedException();
        }
    }
}
