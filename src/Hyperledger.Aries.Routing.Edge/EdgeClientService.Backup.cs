using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Decorators.Attachments;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Indy.CryptoApi;
using Multiformats.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Indy.WalletApi;

namespace Hyperledger.Aries.Routing.Edge
{
    public partial class EdgeClientService : IEdgeClientService
    {
        /// <inheritdoc />
        public async Task<string> CreateBackupAsync(IAgentContext context, string seed)
        {
            if (seed.Length != 32)
            {
                throw new ArgumentException($"{nameof(seed)} should be 32 characters");
            }
            
            var path = Path.Combine(Path.GetTempPath(), seed);
            
            var provRecord = await provisioningService.GetProvisioningAsync(context.Wallet);
            
            var publicKey = provRecord.GetTag("backup_key"); 
            if (string.IsNullOrEmpty(publicKey))
            {
                publicKey = await Crypto.CreateKeyAsync(context.Wallet, new {seed}.ToJson());
                provRecord.SetTag("backup_key", publicKey);
                await walletRecordService.UpdateAsync(context.Wallet, provRecord);
            }

            var json = new { path, key = seed }.ToJson();

            await context.Wallet.ExportAsync(json);

            var bytesArray = await Task.Run(() => File.ReadAllBytes(path));
            var signedBytesArray = await Crypto.SignAsync(context.Wallet, publicKey, bytesArray);

            var payload = bytesArray.ToBase64String();

            var backupMessage = new StoreBackupAgentMessage
            {
                BackupId = publicKey,
                PayloadSignature = signedBytesArray.ToBase64String(),
                Payload = new List<Attachment>()
                {
                    new Attachment
                    {
                        Id = "libindy-backup-request-0",
                        MimeType = CredentialMimeTypes.ApplicationJsonMimeType,
                        Data = new AttachmentContent
                        {
                            Base64 = payload
                        }
                    }
                }
            };

            var connection = await GetMediatorConnectionAsync(context).ConfigureAwait(false);

            if (connection == null)
                throw new AriesFrameworkException(ErrorCode.RecordNotFound,
                    "Couldn't locate a connection to mediator agent");

            await messageService
                .SendReceiveAsync<StoreBackupResponseAgentMessage>(context.Wallet, backupMessage, connection)
                .ConfigureAwait(false);
            return publicKey;
        }

        /// <inheritdoc />
        public async Task<List<Attachment>> RetrieveBackupAsync(IAgentContext context, string seed, DateTimeOffset offset = default)
        {
            var provRecord = await provisioningService.GetProvisioningAsync(context.Wallet);
            var publicKey = provRecord.GetTag("backup_key");
            if (string.IsNullOrEmpty(publicKey))
            {
                throw new ArgumentException("No such key exists");
            }

            var decodedKey = Multibase.Base58.Decode(publicKey);
            var publicKeySigned = await Crypto.SignAsync(context.Wallet, publicKey, decodedKey);

            var retrieveBackupResponseMessage = new RetrieveBackupAgentMessage()
            {
                BackupId = publicKey,
                Signature = publicKeySigned.ToBase64String()
            };

            var connection = await GetMediatorConnectionAsync(context).ConfigureAwait(false);
            if (connection == null)
                throw new AriesFrameworkException(ErrorCode.RecordNotFound, "Couldn't locate a connection to mediator agent");

            var response = await messageService.SendReceiveAsync<RetrieveBackupResponseAgentMessage>(context.Wallet, retrieveBackupResponseMessage, connection).ConfigureAwait(false);
            return response.Payload;
        }
        
        /// <inheritdoc />
        public async Task RestoreFromBackupAsync(IAgentContext sourceContext,
            string seed, 
            List<Attachment> backupData)
        {
            var tempWalletPath = Path.Combine(Path.GetTempPath(), seed);
            var walletBase64 = backupData.First().Data.Base64;
            var walletToRestoreInBytes = walletBase64.GetBytesFromBase64();
            
            await Task.Run(() => File.WriteAllBytes(tempWalletPath, walletToRestoreInBytes));
            
            var json = new { path = tempWalletPath, key = seed }.ToJson();

            await walletService.DeleteWalletAsync(agentoptions.WalletConfiguration, agentoptions.WalletCredentials);
            
            // Add 1 sec delay to allow filesystem to catch up
            await Task.Delay(TimeSpan.FromSeconds(1));

            await Wallet.ImportAsync(agentoptions.WalletConfiguration.ToJson(), agentoptions.WalletCredentials.ToJson(), json);
        }

        /// <inheritdoc />
        public async Task<List<string>> ListBackupsAsync(IAgentContext context)
        {
            var provRecord = await provisioningService.GetProvisioningAsync(context.Wallet);
            var publicKey = provRecord.GetTag("backup_key");
            if (string.IsNullOrEmpty(publicKey))
            {
                throw new ArgumentException("No such key exists");
            }
            
            var listBackupsMessage = new ListBackupsAgentMessage()
            {
                BackupId = publicKey,
            };

            var connection = await GetMediatorConnectionAsync(context).ConfigureAwait(false);
            if (connection == null)
                throw new AriesFrameworkException(ErrorCode.RecordNotFound, "Couldn't locate a connection to mediator agent");

            var response = await messageService.SendReceiveAsync<ListBackupsResponseAgentMessage>(context.Wallet, listBackupsMessage, connection).ConfigureAwait(false);
            return response.BackupList.ToList();
        }
    }
}
