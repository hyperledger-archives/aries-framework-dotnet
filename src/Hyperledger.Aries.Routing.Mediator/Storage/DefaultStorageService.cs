using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Decorators.Attachments;
using Hyperledger.Aries.Extensions;
using Hyperledger.Indy.WalletApi;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Routing.Mediator.Storage
{
    public class DefaultStorageService : IStorageService
    {
        private readonly AgentOptions _agentOptions;
        private string StorageDirectory { get; set; }

        public DefaultStorageService(IOptions<AgentOptions> agentOptions)
        {
            _agentOptions = agentOptions.Value;
            StorageDirectory = Path.Combine(Path.GetTempPath(), "AriesWallets");
            if (!Directory.Exists(StorageDirectory))
            {
                Directory.CreateDirectory(StorageDirectory);
            }
        }

        public async Task<string> StoreBackupAsync(string backupId, Attachment[] payload)
        {
            var backupDir = Path.Combine(StorageDirectory, backupId);

            if (!Directory.Exists(backupDir))
            {
                Directory.CreateDirectory(backupDir);
            }

            var numBackup = Directory.GetFiles(backupDir).Length;

            var path = Path.Combine(backupDir, $"{numBackup+1}");
            
            if (payload is null || payload.Length == 0 || payload.Length > 1)
            {
                throw new ArgumentException("Invalid payload", nameof(payload));
            }
            
            // ReSharper disable once PossibleNullReferenceException
            var payloadInBytes = payload.FirstOrDefault().Data.Base64.GetBytesFromBase64();

            await Task.Run(() =>
                File.WriteAllBytes(path, payloadInBytes)).ConfigureAwait(false);
            
            return new DateTimeOffset(DateTime.UtcNow).ToString();
        }

        public Task<byte[]> RetrieveBackupAsync(string backupId)
        {
            var backupDir = GetWalletPath(backupId);
            
            if (!Directory.Exists(backupDir))
                throw new FileNotFoundException($"Wallet for key {backupId} was not found.");

            var path = Directory.GetFiles(backupDir).Last();

            return RetrieveWallet(backupId, path);
        }

        public Task<IEnumerable<string>> ListBackupsAsync(string backupId)
        {
            return Task.FromResult(Directory.GetFiles(GetWalletPath(backupId)).AsEnumerable());
        }

        public Task<byte[]> RetrieveBackupAsync(string backupId, DateTimeOffset dateTimeOffset)
        {
            if (dateTimeOffset == default)
                return RetrieveBackupAsync(backupId);

            var backupDir = GetWalletPath(backupId);

            var path = Directory.GetFiles(backupDir).Last(x => File.GetCreationTimeUtc(x) > dateTimeOffset.ToUniversalTime());

            return RetrieveWallet(backupId, path);
        }

        private async Task<byte[]> RetrieveWallet(string backupId, string path)
        {
            var json = JsonConvert.SerializeObject(new { path, key = backupId });
            var conf = JsonConvert.SerializeObject(_agentOptions.WalletConfiguration);
            var cred = JsonConvert.SerializeObject(_agentOptions.WalletCredentials);

            // should be done on client side()
            await Wallet.ImportAsync(conf, cred, json);
            return await Task.Run(() => File.ReadAllBytes(path));
        }
        
        private string GetWalletPath(string key) => Path.Combine(StorageDirectory, key);
    }
}