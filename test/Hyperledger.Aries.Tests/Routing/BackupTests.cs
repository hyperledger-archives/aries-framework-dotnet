using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Routing;
using Hyperledger.TestHarness.Mock;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Decorators.Attachments;
using Hyperledger.Aries.Storage;
using Hyperledger.Indy.DidApi;
using Microsoft.Extensions.Options;
using Xunit;

namespace Hyperledger.Aries.Tests.Routing
{
    public class BackupTests : IAsyncLifetime
    {
        public InProcAgent.PairedAgents Pair { get; private set; }
        
        public IEdgeClientService EdgeClient { get; private set; }
        public IAgentContext EdgeContext { get; private set; }
        public AgentOptions AgentOptions { get; private set; }
        public IAgentContext MediatorContext { get; private set; }
        public IWalletService WalletService { get; private set; }

        public async Task DisposeAsync()
        {
            await Pair.Agent1.DisposeAsync();
            await Pair.Agent2.DisposeAsync();
        }

        public async Task InitializeAsync()
        {
            // Agent1 - Mediator
            // Agent2 - Edge
            Pair = await InProcAgent.CreatePairedWithRoutingAsync();

            // WalletService = Pair.Agent2.Host.Services.GetRequiredService<IWalletService>();
            EdgeClient = Pair.Agent2.Host.Services.GetRequiredService<IEdgeClientService>();
            WalletService = Pair.Agent2.Host.Services.GetRequiredService<IWalletService>();
            AgentOptions = Pair.Agent2.Host.Services.GetRequiredService<IOptions<AgentOptions>>().Value;
            EdgeContext = Pair.Agent2.Context;
            MediatorContext = Pair.Agent1.Context;
        }

        [Fact(DisplayName = "Create backup with default seed")]
        public async Task CreateBackup()
        {
            var seed = "00000000000000000000000000000000";

            var path = SetupDirectoriesAndReturnPath(seed);
            
            await EdgeClient.CreateBackupAsync(EdgeContext, seed);
            var numDirsAfterBackup = Directory.GetDirectories(path).Length;
            var walletDir = Directory.GetDirectories(path).First();
            var backupDir = Directory.GetDirectories(walletDir).First();
            var backedUpWallet = Directory.GetFiles(backupDir).First();
            
            Assert.True(Directory.Exists(path));
            Assert.True(numDirsAfterBackup > 0);
            Assert.True(File.Exists(backedUpWallet));
        }
        
        [Fact(DisplayName = "Create backup with shorter seed throws ArgumentException")]
        public async Task CreateBackupWithShortSeed()
        {
            var seed = "11112222";
            SetupDirectoriesAndReturnPath(seed);
            
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => EdgeClient.CreateBackupAsync(EdgeContext, seed));
            Assert.Equal(ex.Message, $"{nameof(seed)} should be 32 characters");
        }

        [Fact(DisplayName = "Get a list of available backups")]
        public async Task ListBackups()
        {
            // change backupId to be retrieved from provisioning service
            // Wait one second
            await Task.Delay(TimeSpan.FromSeconds(1));
            
            var seed = "00000000000000000000000000000000";
            SetupDirectoriesAndReturnPath(seed);

            await EdgeClient.CreateBackupAsync(EdgeContext, seed);
            var result = await EdgeClient.ListBackupsAsync(EdgeContext);

            Assert.NotEmpty(result);
        }

        [Fact(DisplayName = "Retrieve latest backup")]
        public async Task RetrieveLatestBackup()
        {
            var seed = "00000000000000000000000000000000";

            SetupDirectoriesAndReturnPath(seed);
            await EdgeClient.CreateBackupAsync(EdgeContext, seed);
            
            var result = await EdgeClient.RetrieveBackupAsync(EdgeContext, seed);
            
            Assert.NotEmpty(result);
            Assert.IsType<Attachment>(result.First());
        }

        [Fact(DisplayName = "Restore edge agent from backup")]
        public async Task RestoreAgentFromBackup()
        {
            var seed = "00000000000000000000000000000000";
            var path = SetupDirectoriesAndReturnPath(seed);
            var myDid = await Did.CreateAndStoreMyDidAsync(EdgeContext.Wallet, "{}");
            await EdgeClient.CreateBackupAsync(EdgeContext, seed);
            // Create a DID that we will retrieve and compare from imported wallet
            
            var attachments = await EdgeClient.RetrieveBackupAsync(EdgeContext, seed);
            await EdgeClient.RestoreFromBackupAsync(EdgeContext, seed, attachments);

            var newWallet = await WalletService.GetWalletAsync(AgentOptions.WalletConfiguration, AgentOptions.WalletCredentials);
            
            var myKey = await Did.KeyForLocalDidAsync(newWallet, myDid.Did);
            Assert.Equal(myKey, myDid.VerKey);
        }

        private string SetupDirectoriesAndReturnPath(string seed)
        {
            var edgeWallet = Path.Combine(Path.GetTempPath(), seed);

            if (File.Exists(edgeWallet))
            {
                File.Delete(edgeWallet);
            }
            
            var path = Path.Combine(Path.GetTempPath(), "AriesBackups");

            var walletDirExists = Directory.Exists(path);

            if (walletDirExists)
            {
                Directory.Delete(path, true);
            }

            return path;
        }
    }
}
