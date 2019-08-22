using System.Threading.Tasks;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Models.Ledger;
using AgentFramework.Core.Models.Payments;
using AgentFramework.Core.Models.Records;
using AgentFramework.TestHarness;
using Xunit;

namespace AgentFramework.Core.Tests.Payments
{
    public class TransferTests : TestSingleWallet
    {
        [Fact(DisplayName = "Create random payment address for Sovrin method")]
        public async Task CreateSovrinPaymentAddress()
        {
            var address = await paymentService.CreatePaymentAddressAsync(Context);

            Assert.NotNull(address);
            Assert.NotNull(address.Address);
        }

        [Fact(DisplayName = "Mint Sovrin tokens")]
        public async Task MintSovrinTokens()
        {
            var address = await paymentService.CreatePaymentAddressAsync(Context);

            Assert.Equal(0UL, address.Balance);

            var request = await Hyperledger.Indy.PaymentsApi.Payments.BuildMintRequestAsync(Context.Wallet, Trustee.Did,
                new[] {new IndyPaymentOutputSource {Recipient = address.Address, Amount = 42UL}}.ToJson(), null);

            var mintResponse = await TrusteeMultiSignAndSubmitRequestAsync(request.Result);

            await paymentService.RefreshBalanceAsync(Context, address);

            Assert.Equal(42UL, address.Balance);
        }

        [Fact(DisplayName = "Send multiple payments to multiple addresses and check overspend")]
        public async Task SendRecurringPaymentsAndCheckOverSpend()
        {
            const int addressCount = 3;

            const ulong beginningAmount = 20;
            const ulong transferAmount = 5;

            var fee = await paymentService.GetTransactionFeeAsync(Context, TransactionTypes.XFER_PUBLIC);
            var address = new PaymentAddressRecord[addressCount];

            // check all addresses for 0 beginning
            for (var i = 0; i < addressCount; i++)
            {
                address[i] = await paymentService.CreatePaymentAddressAsync(Context);
                Assert.Equal(0UL, address[i].Balance);
            }

            // Mint tokens to the address to fund initially
            var request = await Hyperledger.Indy.PaymentsApi.Payments.BuildMintRequestAsync(Context.Wallet, Trustee.Did,
                new[] {new {recipient = address[0].Address, amount = beginningAmount}}.ToJson(), null);
            await TrusteeMultiSignAndSubmitRequestAsync(request.Result);

            // check beginning balance
            await paymentService.RefreshBalanceAsync(Context, address[0]);
            Assert.Equal(address[0].Balance, beginningAmount);

            //transfer an amount of tokens to another address twice in a row
            // --- Payment 1 ---
            var expectedBalX = address[0].Balance - transferAmount;
            var expectedBalY = address[1].Balance + transferAmount - fee;
            // Create payment record and make payment
            var paymentRecord = new PaymentRecord
            {
                Address = address[1].Address,
                Amount = transferAmount
            };
            await recordService.AddAsync(Context.Wallet, paymentRecord);

            // transfer tokens between two agents
            await paymentService.MakePaymentAsync(Context, paymentRecord, address[0]);

            await paymentService.RefreshBalanceAsync(Context, address[0]);
            await paymentService.RefreshBalanceAsync(Context, address[1]);

            Assert.Equal(expectedBalX, address[0].Balance);
            Assert.Equal(expectedBalY, address[1].Balance);


            // --- Payment 2 ---
            expectedBalX = address[0].Balance - transferAmount;
            expectedBalY = address[1].Balance + transferAmount - fee;
            // Create payment record and make payment
            var paymentRecord1 = new PaymentRecord
            {
                Address = address[1].Address,
                Amount = transferAmount
            };
            await recordService.AddAsync(Context.Wallet, paymentRecord1);

            // transfer tokens between two agents
            await paymentService.MakePaymentAsync(Context, paymentRecord1, address[0]);

            await paymentService.RefreshBalanceAsync(Context, address[0]);
            await paymentService.RefreshBalanceAsync(Context, address[1]);

            Assert.Equal(expectedBalX, address[0].Balance);
            Assert.Equal(expectedBalY, address[1].Balance);


            // --- payment 3 --- from recipient to new address
            expectedBalX = address[1].Balance - transferAmount;
            expectedBalY = address[2].Balance + transferAmount - fee;

            var paymentRecord2 = new PaymentRecord
            {
                Address = address[2].Address,
                Amount = transferAmount
            };
            await recordService.AddAsync(Context.Wallet, paymentRecord2);

            // transfer tokens from second to third agent
            await paymentService.MakePaymentAsync(Context, paymentRecord2, address[1]);
            await paymentService.RefreshBalanceAsync(Context, address[1]);
            await paymentService.RefreshBalanceAsync(Context, address[2]);

            Assert.Equal(expectedBalX, address[1].Balance);
            Assert.Equal(expectedBalY, address[2].Balance);


            // --- Overspend Payment ---
            // no balances should change
            expectedBalX = address[0].Balance;
            expectedBalY = address[2].Balance;
            var paymentRecord3 = new PaymentRecord
            {
                Address = address[2].Address,
                Amount = beginningAmount
            };

            // transfer tokens between two agents
            var ex = await Assert.ThrowsAsync<AgentFrameworkException>(async () =>
                await paymentService.MakePaymentAsync(Context, paymentRecord3, address[0]));

            Assert.Equal(ErrorCode.PaymentInsufficientFunds, ex.ErrorCode);

            await paymentService.RefreshBalanceAsync(Context, address[0]);
            await paymentService.RefreshBalanceAsync(Context, address[2]);
            Assert.Equal(expectedBalX, address[0].Balance);
            Assert.Equal(expectedBalY, address[2].Balance);
        }
    }
}