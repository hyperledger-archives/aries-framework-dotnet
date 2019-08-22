using System;
using System.Threading.Tasks;
using AgentFramework.Core.Models.Payments;
using Newtonsoft.Json;
using Stateless;

namespace AgentFramework.Core.Models.Records
{
    /// <summary>
    /// Payment record representing an abstraction of payment workflow as described in aries-rfc
    /// https://github.com/hyperledger/aries-rfcs/tree/master/features/0075-payment-decorators
    /// </summary>
    /// <seealso cref="AgentFramework.Core.Models.Records.RecordBase" />
    public sealed class PaymentRecord : RecordBase
    {
        private PaymentState _state;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentRecord"/> class.
        /// </summary>
        public PaymentRecord()
        {
            Id = Guid.NewGuid().ToString();
            State = PaymentState.None;
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        public override string TypeName => "AF.PaymentRecord";

        /// <summary>
        /// Gets or sets the record associated with this payment.
        /// Ex: CredentialRecord, SchemaRecord, etc.
        /// </summary>
        [JsonIgnore]
        public string RecordId
        {
            get => Get();
            set => Set(value);
        }

        /// <summary>
        /// Gets or sets the receipt for this payment.
        /// Receipt can be represented as UTXO source.
        /// </summary>
        [JsonIgnore]
        public string ReceiptId
        {
            get => Get();
            set => Set(value);
        }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        [JsonIgnore]
        public string Method
        {
            get => Get();
            set => Set(value);
        }

        /// <summary>
        /// Gets or sets the payment address or Payee Id.
        /// </summary>
        [JsonIgnore]
        public string Address
        {
            get => Get();
            set => Set(value);
        }

        /// <summary>
        /// Gets or sets the payment amount
        /// </summary>
        /// <value>
        /// The amount.
        /// </value>
        public ulong Amount { get; set; }

        /// <summary>
        /// Gets or sets the state of this record.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public PaymentState State
        {
            get => _state;
            set => Set(value, ref _state);
        }

        /// <summary>
        /// Gets or sets the payment details for this payment
        /// </summary>
        public PaymentDetails Details { get; set; }

        /// <summary>
        /// Gets or sets the connection identifier.
        /// </summary>
        /// <value>
        /// The connection identifier.
        /// </value>
        [JsonIgnore]
        public string ConnectionId
        {
            get => Get();
            set => Set(value);
        }

        /// <summary>
        /// Gets or sets the reference identifier.
        /// </summary>
        /// <value>
        /// The reference identifier.
        /// </value>
        [JsonIgnore]
        public string ReferenceId
        {
            get => Get();
            set => Set(value);
        }

        /// <summary>
        /// Triggers the asynchronous.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        /// <returns></returns>
        public Task TriggerAsync(PaymentTrigger trigger) => GetStateMachine().FireAsync(trigger);

        private StateMachine<PaymentState, PaymentTrigger> GetStateMachine()
        {
            var state = new StateMachine<PaymentState, PaymentTrigger>(() => State, x => State = x);
            state.Configure(PaymentState.None).Permit(PaymentTrigger.RequestSent, PaymentState.Requested);
            state.Configure(PaymentState.None).Permit(PaymentTrigger.RequestReceived, PaymentState.RequestReceived);
            state.Configure(PaymentState.None).Permit(PaymentTrigger.ProcessPayment, PaymentState.Paid);
            state.Configure(PaymentState.None).Permit(PaymentTrigger.ReceiptReceived, PaymentState.ReceiptReceived);
            state.Configure(PaymentState.Requested).Permit(PaymentTrigger.ReceiptReceived, PaymentState.ReceiptReceived);
            state.Configure(PaymentState.RequestReceived).Permit(PaymentTrigger.ProcessPayment, PaymentState.Paid);
            return state;
        }
    }

    /// <summary>
    /// Payment state
    /// </summary>
    public enum PaymentState
    {
        /// <summary>
        /// Default state
        /// </summary>
        None = 0,
        /// <summary>
        /// Payment request sent
        /// </summary>
        Requested,
        /// <summary>
        /// Payment request received
        /// </summary>
        RequestReceived,
        /// <summary>
        /// Payment processed
        /// </summary>
        Paid,
        /// <summary>
        /// Payment receipt received
        /// </summary>
        ReceiptReceived
    }

    /// <summary>
    /// 
    /// </summary>
    public enum PaymentTrigger
    {
        /// <summary>
        /// The request sent
        /// </summary>
        RequestSent,
        /// <summary>
        /// The request received
        /// </summary>
        RequestReceived,
        /// <summary>
        /// The process payment
        /// </summary>
        ProcessPayment,
        /// <summary>
        /// The receipt received
        /// </summary>
        ReceiptReceived
    }
}
