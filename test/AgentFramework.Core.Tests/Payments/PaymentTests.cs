using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Models.Ledger;
using AgentFramework.Core.Models.Records;
using AgentFramework.TestHarness;
using AgentFramework.Payments.SovrinToken;
using Hyperledger.Indy.LedgerApi;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Xunit;
using Indy = Hyperledger.Indy.PaymentsApi;



namespace AgentFramework.Core.Tests.Payments
{
    public class FeesTests : TestSingleWallet
    {
        [Fact(DisplayName = "Get fees from ledger for schema transaction type")]
        public async Task GetTransactionFeesAsync()
        {
            await paymentService.GetTransactionFeeAsync(Context, TransactionTypes.SCHEMA);

            Assert.True(true);
        }
        
        [Fact(DisplayName = "Create schema with non-zero fees")]
        public async Task CreateSchemaWithFeesAsync()
        {
            var schemaService = Host.Services.GetService<ISchemaService>();
            var prov = await provisioningService.GetProvisioningAsync(Context.Wallet);

            await PromoteTrustAnchor(prov.IssuerDid, prov.IssuerVerkey);

            await FundDefaultAccountAsync(10);
            await SetFeesForSchemaTransactionsAsync(3);

            var schemaId = await schemaService.CreateSchemaAsync(Context, $"test{Guid.NewGuid().ToString("N")}", "1.0", new[] { "name-one" });

            Assert.NotNull(schemaId);

            var address = await recordService.GetAsync<PaymentAddressRecord>(Context.Wallet, prov.DefaultPaymentAddressId);

            Assert.Equal(7UL, address.Balance);

            await UnsetFeesForSchemaTransactionsAsync();
        }

        [Fact(DisplayName = "Transfer funds between Sovrin addresses with ledger fees")]
        public async Task TransferFundsAsync()
        {
            // Generate from address
            var addressFrom = await paymentService.CreatePaymentAddressAsync(Context);
            await SetFeesForPublicXferTransactionsAsync(2);

            // Mint tokens to the address to fund initially
            var request = await Indy.Payments.BuildMintRequestAsync(Context.Wallet, Trustee.Did,
                new[] { new { recipient = addressFrom.Address, amount = 15 } }.ToJson(), null);
            await TrusteeMultiSignAndSubmitRequestAsync(request.Result);

            // Generate destination address
            var addressTo = await paymentService.CreatePaymentAddressAsync(Context);

            // Create payment record and make payment
            var paymentRecord = new PaymentRecord
            {
                Address = addressTo.Address,
                Amount = 10
            };
            await recordService.AddAsync(Context.Wallet, paymentRecord);
            await paymentService.MakePaymentAsync(Context, paymentRecord, addressFrom);

            var fee = await paymentService.GetTransactionFeeAsync(Context, TransactionTypes.XFER_PUBLIC);
            Assert.Equal(2UL, fee);

            await paymentService.RefreshBalanceAsync(Context, addressFrom);
            await paymentService.RefreshBalanceAsync(Context, addressTo);

            Assert.Equal(10UL, addressTo.Balance);
            Assert.Equal(3UL, addressFrom.Balance);

            await UnsetFeesForPublicXferTransactionsAsync();
        }

        [Fact(DisplayName = "Get auth rules from the ledger")]
        public async Task GetAuthRules()
        {
            var ledgerService = Host.Services.GetService<ILedgerService>();

            var rules = await ledgerService.LookupAuthorizationRulesAsync(await Context.Pool);

            Assert.NotNull(rules);
            Assert.True(rules.Any());
            Assert.True(rules.Count > 0);
        }
        
        public async Task SetFeesForSchemaTransactionsAsync(ulong amount)
        {
            var request = await Indy.Payments.BuildSetTxnFeesRequestAsync(Context.Wallet, Trustee.Did, TokenConfiguration.MethodName,
                new Dictionary<string, ulong>
                {
                                { "fees_for_schema", amount }
                }.ToJson());
            await TrusteeMultiSignAndSubmitRequestAsync(request);

            request = await Ledger.BuildAuthRuleRequestAsync(Trustee.Did, "101", "ADD", "*", "*", "*", new
            {
                constraint_id = "OR",
                auth_constraints = new[] {
                    new {
                        metadata = new {
                            fees = "fees_for_schema"
                        },
                      constraint_id = "ROLE",
                      need_to_be_owner = false,
                      role = "0",
                      sig_count = 1
                    },
                    new {
                        metadata= new {
                            fees = "fees_for_schema"
                        },
                      constraint_id= "ROLE",
                      need_to_be_owner= false,
                      role= "2",
                      sig_count= 1
                    },
                    new {
                        metadata= new {
                            fees = "fees_for_schema"
                        },
                      constraint_id= "ROLE",
                      need_to_be_owner= false,
                      role= "101",
                      sig_count= 1
                    }
                }
            }.ToJson());
            await TrusteeMultiSignAndSubmitRequestAsync(request);
        }

        public async Task SetFeesForPublicXferTransactionsAsync(ulong amount)
        {
            var request = await Indy.Payments.BuildSetTxnFeesRequestAsync(Context.Wallet, Trustee.Did, TokenConfiguration.MethodName,
                new Dictionary<string, ulong>
                {
                                { "fees_for_xfer", amount }
                }.ToJson());
            await TrusteeMultiSignAndSubmitRequestAsync(request);

            request = await Ledger.BuildAuthRuleRequestAsync(Trustee.Did, "10001", "ADD", "*", "*", "*", new
            {
                constraint_id = "ROLE",
                metadata = new {
                    fees = "fees_for_xfer"
                },
                need_to_be_owner = false,
                role = "*",
                sig_count = 0
            }.ToJson());
            await TrusteeMultiSignAndSubmitRequestAsync(request);
        }

        public async Task UnsetFeesForPublicXferTransactionsAsync()
        {
            var request = await Indy.Payments.BuildSetTxnFeesRequestAsync(Context.Wallet, Trustee.Did, TokenConfiguration.MethodName,
                new Dictionary<string, ulong>
                {
                                { "fees_for_xfer", 0 }
                }.ToJson());
            await TrusteeMultiSignAndSubmitRequestAsync(request);

            request = await Ledger.BuildAuthRuleRequestAsync(Trustee.Did, "10001", "ADD", "*", "*", "*", new
            {
                constraint_id = "ROLE",
                metadata = new { },
                need_to_be_owner = false,
                role = "*",
                sig_count = 0
            }.ToJson());
            await TrusteeMultiSignAndSubmitRequestAsync(request);
        }

        public async Task UnsetFeesForSchemaTransactionsAsync()
        {
            var request = await Indy.Payments.BuildSetTxnFeesRequestAsync(Context.Wallet, Trustee.Did, TokenConfiguration.MethodName,
                new Dictionary<string, ulong>
                {
                                { "fees_for_schema", 0 }
                }.ToJson());
            await TrusteeMultiSignAndSubmitRequestAsync(request);

            request = await Ledger.BuildAuthRuleRequestAsync(Trustee.Did, "101", "ADD", "*", "*", "*", new
            {
                constraint_id = "OR",
                auth_constraints = new[] {
                    new {
                        metadata = new {
                        },
                      constraint_id = "ROLE",
                      need_to_be_owner = false,
                      role = "0",
                      sig_count = 1
                    },
                    new {
                        metadata= new {
                        },
                      constraint_id= "ROLE",
                      need_to_be_owner= false,
                      role= "2",
                      sig_count= 1
                    },
                    new {
                        metadata= new {
                        },
                      constraint_id= "ROLE",
                      need_to_be_owner= false,
                      role= "101",
                      sig_count= 1
                    }
                }
            }.ToJson());
            await TrusteeMultiSignAndSubmitRequestAsync(request);
        }

        [Fact(DisplayName = "Set transaction fees")]
        public async Task SetTransactionFees()
        {
            var request = await Indy.Payments.BuildSetTxnFeesRequestAsync(Context.Wallet, Trustee.Did, TokenConfiguration.MethodName,
                new Dictionary<string, ulong>
                {
                    { "101", 1 },
                    { "10001", 2 }
                }.ToJson());
            var response = await TrusteeMultiSignAndSubmitRequestAsync(request);
            var jResponse = JObject.Parse(response);

            Assert.Equal("REPLY", jResponse["op"].ToString());

            // Cleanup and revert back fees to 0
            request = await Indy.Payments.BuildSetTxnFeesRequestAsync(Context.Wallet, Trustee.Did, TokenConfiguration.MethodName,
                new Dictionary<string, ulong>
                {
                    { "101", 0 },
                    { "10001", 0 }
                }.ToJson());
            await TrusteeMultiSignAndSubmitRequestAsync(request);
        }
    }
}
