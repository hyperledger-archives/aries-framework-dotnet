using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Routing;
using Hyperledger.TestHarness.Mock;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Hyperledger.Aries.Tests.Routing
{
    public class BackupTests : IAsyncLifetime
    {
        public InProcAgent.PairedAgents Pair { get; private set; }
        public IEdgeClientService EdgeClient { get; private set; }
        public IAgentContext EdgeContext { get; private set; }
        public IAgentContext MediatorContext { get; private set; }

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
            EdgeClient = Pair.Agent2.Host.Services.GetRequiredService<IEdgeClientService>();
            EdgeContext = Pair.Agent2.Context;
            MediatorContext = Pair.Agent1.Context;
        }

        [Fact(DisplayName = "Create backup with default seed")]
        public async Task CreateBackup()
        {
            var seed = "00000000000000000000000000000000";

            var edgeWallet = Path.Combine(Path.GetTempPath(), seed);

            if (File.Exists(edgeWallet))
            {
                File.Delete(edgeWallet);
            }
            
            var path = Path.Combine(Path.GetTempPath(), "AriesWallets");

            var walletDirExists = Directory.Exists(path);

            if (walletDirExists)
            {
                Directory.Delete(path, true);
            }
            
            var r = await EdgeClient.CreateBackupAsync(EdgeContext, seed);
            var numDirsAfterBackup = Directory.GetDirectories(path).Length;
            var walletDir = Directory.GetDirectories(path).First();
            var backupDir = Directory.GetDirectories(walletDir).First();
            var backedUpWallet = Directory.GetFiles(backupDir).First();
            
            Assert.True(Directory.Exists(path));
            Assert.True(numDirsAfterBackup > 0);
            Assert.True(File.Exists(backedUpWallet));
        }
        
        [Fact(DisplayName = "Create backup with shorter seed")]
        public async Task CreateBackupWithShortSeed()
        {
            var seed = "11112222";

            var edgeWallet = Path.Combine(Path.GetTempPath(), seed);

            if (File.Exists(edgeWallet))
            {
                File.Delete(edgeWallet);
            }
            
            var path = Path.Combine(Path.GetTempPath(), "AriesWallets");

            var walletDirExists = Directory.Exists(path);

            if (walletDirExists)
            {
                Directory.Delete(path, true);
            }
            
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => EdgeClient.CreateBackupAsync(EdgeContext, seed));
            Assert.Equal(ex.Message, $"{nameof(seed)} should be 32 characters");
        }

        [Fact(DisplayName = "Get a list of available backups")]
        public async Task ListBackups()
        {
            var seed = "00000000000000000000000000000000";

            await EdgeClient.CreateBackupAsync(EdgeContext, seed);

            // Wait one second
            await Task.Delay(TimeSpan.FromSeconds(1));

            await EdgeClient.CreateBackupAsync(EdgeContext, seed);

            var result = await EdgeClient.ListBackupsAsync(EdgeContext, seed);

            Assert.NotEmpty(result);
            // TODO: Add response to ListBackups

            // TODO: Add assertsions
        }

        [Fact(DisplayName = "Retrieve latest backup")]
        public async Task RetrieveLatestBackup()
        {
            var seed = "00000000000000000000000000000000";

            await EdgeClient.RetrieveBackupAsync(EdgeContext, seed);

            // TODO: Add response

            // TODO: Add assertsions
        }

        [Fact(DisplayName = "Restore edge agent from backup")]
        public async Task RestoreAgentFromBackup()
        {
            var seed = "00000000000000000000000000000000";

            var backupData = await EdgeClient.RetrieveBackupAsync(EdgeContext, seed);

            await EdgeClient.RestoreFromBackupAsync(EdgeContext, seed, backupData);

            // TODO: Add response

            // TODO: Add assertsions
        }
    }
}
