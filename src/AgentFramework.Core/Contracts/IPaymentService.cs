using System.Threading.Tasks;
using AgentFramework.Core.Messages;
using AgentFramework.Core.Models.Payments;
using AgentFramework.Core.Models.Records;

namespace AgentFramework.Core.Contracts
{
    /// <summary>
    /// Payment Service Interface
    /// </summary>
    public interface IPaymentService
    {
        /// <summary>
        /// Gets the default payment address for the given agent context.
        /// </summary>
        /// <param name="agentContext"></param>
        /// <returns></returns>
        Task<PaymentAddressRecord> GetDefaultPaymentAddressAsync(IAgentContext agentContext);

        /// <summary>
        /// Sets the given address as default payment address.
        /// </summary>
        /// <param name="agentContext"></param>
        /// <param name="addressRecord"></param>
        /// <returns></returns>
        Task SetDefaultPaymentAddressAsync(IAgentContext agentContext, PaymentAddressRecord addressRecord);

        /// <summary>
        /// Creates a new payment address record.
        /// </summary>
        /// <param name="agentContext"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        Task<PaymentAddressRecord> CreatePaymentAddressAsync(IAgentContext agentContext, AddressOptions configuration = null);

        /// <summary>
        /// Attaches a payment request to the given agent message.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="agentMessage"></param>
        /// <param name="details">The details of the payment</param>
        /// <param name="payeeAddress">The address this payment will be processed to</param>
        /// <returns>Payment record that can be used to reference this payment</returns>
        Task<PaymentRecord> AttachPaymentRequestAsync(IAgentContext context, AgentMessage agentMessage, PaymentDetails details, PaymentAddressRecord payeeAddress = null);

        /// <summary>
        /// Attach a payment receipt to the given agent message based on the payment record.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="agentMessage"></param>
        /// <param name="paymentRecord"></param>
        void AttachPaymentReceipt(IAgentContext context, AgentMessage agentMessage, PaymentRecord paymentRecord);

        /// <summary>
        /// Makes a payment for the given payment record.
        /// </summary>
        /// <param name="agentContext"></param>
        /// <param name="paymentRecord">The payment record</param>
        /// <param name="addressRecord">The address to use to make this payment from. If null,
        /// the default payment address will be used</param>
        /// <returns></returns>
        Task MakePaymentAsync(IAgentContext agentContext, PaymentRecord paymentRecord, PaymentAddressRecord addressRecord = null);

        /// <summary>
        /// Refresh the payment address sources from the ledger.
        /// </summary>
        /// <param name="agentContext"></param>
        /// <param name="paymentAddress">The address to refresh. If null, the default payment address will be used</param>
        /// <returns></returns>
        Task RefreshBalanceAsync(IAgentContext agentContext, PaymentAddressRecord paymentAddress = null);

        /// <summary>
        /// Creates a payment info object for the given transaction type. The payment info makes auto discovery of the
        /// fees required by querying the ledger.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="transactionType"></param>
        /// <param name="addressRecord"></param>
        /// <returns></returns>
        Task<TransactionCost> GetTransactionCostAsync(IAgentContext context, string transactionType, PaymentAddressRecord addressRecord = null);

        /// <summary>
        /// Gets the fees associated with a given transaction type
        /// </summary>
        /// <param name="agentContext"></param>
        /// <param name="transactionType"></param>
        /// <returns></returns>
        Task<ulong> GetTransactionFeeAsync(IAgentContext agentContext, string transactionType);

        /// <summary>
        /// Verifies the payment record on the ledger by comparing the receipts and amounts
        /// </summary>
        /// <param name="context"></param>
        /// <param name="paymentRecord">The payment record to verify</param>
        /// <returns></returns>
        Task<bool> VerifyPaymentAsync(IAgentContext context, PaymentRecord paymentRecord);
    }
}
