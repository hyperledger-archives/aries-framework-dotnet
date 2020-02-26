using System;
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

        public async Task<string> SaveWallet(string key, Attachment[] payload)
        {
            var path = Path.Combine(StorageDirectory, key);
            if (payload is null || payload.Length == 0 || payload.Length > 1)
            {
                throw new ArgumentException("Invalid payload", nameof(payload));
            }
            
            // ReSharper disable once PossibleNullReferenceException
            var payloadInBytes = payload.FirstOrDefault().Data.Base64.GetBytesFromBase64();

            await Task.Run(() =>
                File.WriteAllBytes(path, payloadInBytes)).ConfigureAwait(false);
            
            return key;
        }

        public async Task<byte[]> RetrieveWallet(string key)
        {
            var path = GetWalletPath(key);
            
            if (!File.Exists(path))
                throw new FileNotFoundException($"Wallet with key {key} was not found.");

            var json = JsonConvert.SerializeObject(new { path, key });
            var conf = JsonConvert.SerializeObject(_agentOptions.WalletConfiguration);
            var cred = JsonConvert.SerializeObject(_agentOptions.WalletCredentials);

            // should be done on client side()
            await Wallet.ImportAsync(conf, cred, json);
            return await Task.Run(() => File.ReadAllBytes(path));
        }

        private string GetWalletPath(string key) => Path.Combine(StorageDirectory, key);
    }
}