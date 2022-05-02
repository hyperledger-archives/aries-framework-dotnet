using System;
using System.IO;
using System.Threading.Tasks;
using Hyperledger.Aries;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Payments;
using Hyperledger.Aries.Storage;
using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.LedgerApi;
using Hyperledger.Indy.PoolApi;
using Hyperledger.Indy.WalletApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Xunit;
using IndyPayments = Hyperledger.Indy.PaymentsApi.Payments;

namespace Hyperledger.TestHarness
{
    public abstract class TestSingleWallet : IAsyncLifetime
    {
        protected IAgentContext Context { get; private set; }
        public CreateAndStoreMyDidResult Trustee { get; private set; }
        public CreateAndStoreMyDidResult Trustee2 { get; private set; }
        public CreateAndStoreMyDidResult Trustee3 { get; private set; }

        protected IProvisioningService provisioningService;
        protected IWalletRecordService recordService;
        protected IPaymentService paymentService;
        protected ILedgerService ledgerService;

        protected IHost Host { get; private set; }

        protected virtual string GetPoolName() => "TestPool";
        protected virtual string GetIssuerSeed() => null;
        public virtual async Task DisposeAsync()
        {
            var walletOptions = Host.Services.GetService<IOptions<AgentOptions>>().Value;
            await Host.StopAsync();

            await Context.Wallet.CloseAsync();
            await Wallet.DeleteWalletAsync(walletOptions.WalletConfiguration.ToJson(), walletOptions.WalletCredentials.ToJson());
            Host.Dispose();
        }

        /// <summary>
        /// Create a single wallet and enable payments
        /// </summary>
        /// <returns></returns>
        public virtual async Task InitializeAsync()
        {
            Host = new HostBuilder()
                .ConfigureServices(services =>
                {
                    services.Configure<ConsoleLifetimeOptions>(options =>
                        options.SuppressStatusMessages = true);
                    services.AddAriesFramework(builder => builder
                        .RegisterAgent(options =>
                        {
                            options.WalletConfiguration = new WalletConfiguration { Id = Guid.NewGuid().ToString() };
                            options.WalletCredentials = new WalletCredentials { Key = "test" };
                            options.GenesisFilename = Path.GetFullPath("pool_genesis.txn");
                            options.PoolName = GetPoolName();
                            options.EndpointUri = "http://test";
                            options.IssuerKeySeed = GetIssuerSeed();
                        }));
                })
                .Build();

            await Host.StartAsync();
            await Pool.SetProtocolVersionAsync(2);

            Context = await Host.Services.GetService<IAgentProvider>().GetContextAsync();

            Trustee = await Did.CreateAndStoreMyDidAsync(Context.Wallet,
                new { seed = "000000000000000000000000Trustee1" }.ToJson());
            Trustee2 = await PromoteTrustee("000000000000000000000000Trustee2");
            Trustee3 = await PromoteTrustee("000000000000000000000000Trustee3");

            provisioningService = Host.Services.GetService<IProvisioningService>();
            recordService = Host.Services.GetService<IWalletRecordService>();
            paymentService = Host.Services.GetService<IPaymentService>();
            ledgerService = Host.Services.GetService<ILedgerService>();
        }

        async Task<CreateAndStoreMyDidResult> PromoteTrustee(string seed)
        {
            var trustee = await Did.CreateAndStoreMyDidAsync(Context.Wallet, new { seed = seed }.ToJson());

            await Ledger.SignAndSubmitRequestAsync(await Context.Pool, Context.Wallet, Trustee.Did,
                await Ledger.BuildNymRequestAsync(Trustee.Did, trustee.Did, trustee.VerKey, null, "TRUSTEE"));

            return trustee;
        }

        protected async Task PromoteTrustAnchor(string did, string verkey)
        {
            await Ledger.SignAndSubmitRequestAsync(await Context.Pool, Context.Wallet, Trustee.Did,
                await Ledger.BuildNymRequestAsync(Trustee.Did, did, verkey, null, "ENDORSER"));
        }

        protected async Task PromoteTrustAnchor()
        {
            var record = await Host.Services.GetService<IProvisioningService>().GetProvisioningAsync(Context.Wallet);
            if (record.IssuerDid == null || record.IssuerVerkey == null)
                throw new AriesFrameworkException(ErrorCode.InvalidRecordData, "Agent not set up as issuer");

            await Ledger.SignAndSubmitRequestAsync(await Context.Pool, Context.Wallet, Trustee.Did,
                await Ledger.BuildNymRequestAsync(Trustee.Did, record.IssuerDid, record.IssuerVerkey, null, "ENDORSER"));
        }

        protected async Task<string> TrusteeMultiSignAndSubmitRequestAsync(string request)
        {
            var singedRequest1 = await Ledger.MultiSignRequestAsync(Context.Wallet, Trustee.Did, request);
            var singedRequest2 = await Ledger.MultiSignRequestAsync(Context.Wallet, Trustee2.Did, singedRequest1);
            var singedRequest3 = await Ledger.MultiSignRequestAsync(Context.Wallet, Trustee3.Did, singedRequest2);

            return await Ledger.SubmitRequestAsync(await Context.Pool, singedRequest3);
        }

        protected async Task FundDefaultAccountAsync(ulong amount)
        {
            var record = await provisioningService.GetProvisioningAsync(Context.Wallet);
            var addressRecord = await recordService.GetAsync<PaymentAddressRecord>(Context.Wallet, record.DefaultPaymentAddressId);

            // Mint tokens to the address to fund initially
            var request = await IndyPayments.BuildMintRequestAsync(Context.Wallet, Trustee.Did,
                new[] { new { recipient = addressRecord.Address, amount = amount } }.ToJson(), null);
            await TrusteeMultiSignAndSubmitRequestAsync(request.Result);

            await paymentService.RefreshBalanceAsync(Context, addressRecord);
        }

        protected async Task FundAccountAsync(ulong amount, string address)
        {
            var request = await IndyPayments.BuildMintRequestAsync(Context.Wallet, Trustee.Did,
                new[] { new { recipient = address, amount = amount } }.ToJson(), null);
            await TrusteeMultiSignAndSubmitRequestAsync(request.Result);
        }
    }
}
