using System;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;

namespace Hyperledger.Aries.Payments
{
    /// <inheritdoc />
    public class DefaultPaymentService : IPaymentService
    {
        /// <inheritdoc />
        public virtual void AttachPaymentReceipt(IAgentContext context, AgentMessage agentMessage, PaymentRecord paymentRecord)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public virtual Task<PaymentRecord> AttachPaymentRequestAsync(IAgentContext context, AgentMessage agentMessage, PaymentDetails details, PaymentAddressRecord addressRecord = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public virtual Task<PaymentAddressRecord> CreatePaymentAddressAsync(IAgentContext agentContext, AddressOptions configuration = null)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public virtual Task<TransactionCost> GetTransactionCostAsync(IAgentContext context, string transactionType, PaymentAddressRecord addressRecord = null)
        {
            return Task.FromResult<TransactionCost>(null);
        }

        /// <inheritdoc />
        public virtual Task RefreshBalanceAsync(IAgentContext agentContext, PaymentAddressRecord paymentAddress = null)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public virtual Task<PaymentAddressRecord> GetDefaultPaymentAddressAsync(IAgentContext agentContext)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public virtual Task<ulong> GetTransactionFeeAsync(IAgentContext agentContext, string transactionType)
        {
            return Task.FromResult(0UL);
        }

        /// <inheritdoc />
        public virtual Task MakePaymentAsync(IAgentContext agentContext, PaymentRecord paymentRecord, PaymentAddressRecord addressRecord = null)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public virtual Task SetDefaultPaymentAddressAsync(IAgentContext agentContext, PaymentAddressRecord addressRecord)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public virtual Task<bool> VerifyPaymentAsync(IAgentContext context, PaymentRecord paymentRecord)
        {
            throw new NotImplementedException();
        }
    }
}
