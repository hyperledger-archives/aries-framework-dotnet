using System;
using System.Collections.Generic;
using System.Linq;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Models.Payments;
using AgentFramework.Core.Models.Records;

namespace AgentFramework.Payments.SovrinToken
{
    internal static class PaymentUtils
    {
        internal static (IEnumerable<string> inputs, IEnumerable<IndyPaymentOutputSource> outputs) ReconcilePaymentSources(PaymentAddressRecord addressRecord, PaymentRecord paymentRecord, ulong txnFee)
        {
            return ReconcilePaymentSources(addressRecord.Sources, paymentRecord.Address, paymentRecord.Amount, txnFee);
        }

        private static (IEnumerable<string> inputs, IEnumerable<IndyPaymentOutputSource> outputs) ReconcilePaymentSources(IList<IndyPaymentInputSource> inputs, string address, ulong amount, ulong txnFee)
        {
            if (amount == 0) throw new ArgumentOutOfRangeException(nameof(amount), "Cannot make a 0 payment");
            if (address == null) throw new ArgumentNullException(nameof(address), "Address must be specified");


            if (!inputs.Any()) throw new AgentFrameworkException(ErrorCode.PaymentInsufficientFunds, "Insufficient funds");

            return (inputs.Select(x => x.Source), new[]
                {
                    new IndyPaymentOutputSource
                    {
                        Amount = amount,
                        Recipient = address
                    },
                    new IndyPaymentOutputSource
                    {
                        Recipient = inputs.First().PaymentAddress,
                        Amount = Total(inputs) - amount - txnFee
                    }
                });
        }

        private static ulong Total(IEnumerable<IndyPaymentInputSource> source)
        {
            return source.Any() ?
                source.Select(x => x.Amount)
                .Aggregate((x, y) => x + y) : 0;
        }
    }
}