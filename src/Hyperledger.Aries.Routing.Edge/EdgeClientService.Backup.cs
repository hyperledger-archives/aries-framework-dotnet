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
using Hyperledger.Indy.DidApi;
using Hyperledger.Indy;
using Hyperledger.Aries.Configuration;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Routing.Edge
{
    /// <inheritdoc />
    public partial class EdgeClientService : IEdgeClientService
    {
        const string InternalBackupDid = "22222222AriesBackupDid";

        /// <inheritdoc />
        public async Task<string> CreateBackupAsync(IAgentContext context, string seed)
        {
            if (seed.Length != 32)
            {
                throw new ArgumentException($"{nameof(seed)} should be 32 characters");
            }

            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var json = new { path, key = seed }.ToJson();

            await context.Wallet.ExportAsync(json);

            var bytesArray = await Task.Run(() => File.ReadAllBytes(path));

            var backupVerkey = await EnsureBackupKeyAsync(context, seed);
            var signedBytesArray = await Crypto.SignAsync(context.Wallet, backupVerkey, bytesArray);

            var payload = bytesArray.ToBase64String();

            var backupMessage = new StoreBackupAgentMessage
            {
                BackupId = backupVerkey,
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

            File.Delete(path);

            await messageService
                .SendReceiveAsync<StoreBackupResponseAgentMessage>(context, backupMessage, connection)
                .ConfigureAwait(false);
            return backupVerkey;
        }

        private static async Task<string> EnsureBackupKeyAsync(IAgentContext context, string seed)
        {
            try
            {
                var didResult = await Did.CreateAndStoreMyDidAsync(context.Wallet, new
                {
                    did = InternalBackupDid,
                    seed = seed
                }.ToJson());
                return didResult.VerKey;
            }
            catch (IndyException ex) when (ex.SdkErrorCode == 600)
            {
                var key = await Did.ReplaceKeysStartAsync(
                    context.Wallet,
                    InternalBackupDid,
                    new { seed = seed }.ToJson());

                await Did.ReplaceKeysApplyAsync(context.Wallet, InternalBackupDid);
                return key;
            }
        }

        /// <inheritdoc />
        public async Task<List<Attachment>> RetrieveBackupAsync(IAgentContext context, string seed, long offset = default)
        {
            var publicKey = await EnsureBackupKeyAsync(context, seed);

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

            var response = await messageService.SendReceiveAsync<RetrieveBackupResponseAgentMessage>(context, retrieveBackupResponseMessage, connection).ConfigureAwait(false);
            return response.Payload;
        }

        /// <inheritdoc />
        public async Task<AgentOptions> RestoreFromBackupAsync(IAgentContext sourceContext,
            string seed,
            List<Attachment> backupData)
        {
            var tempWalletPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var walletBase64 = backupData.First().Data.Base64;
            var walletToRestoreInBytes = walletBase64.GetBytesFromBase64();

            await Task.Run(() => File.WriteAllBytes(tempWalletPath, walletToRestoreInBytes));

            var oldAgentOptionsString = JsonConvert.SerializeObject(agentoptions);

            var json = new { path = tempWalletPath, key = seed }.ToJson();

            agentoptions.WalletConfiguration.Id = Guid.NewGuid().ToString();
            agentoptions.WalletCredentials.Key = Utils.GenerateRandomAsync(32);

            await Wallet.ImportAsync(agentoptions.WalletConfiguration.ToJson(), agentoptions.WalletCredentials.ToJson(), json);

            // Try delete the old wallet
            try
            {
                var oldAgentOptions = JsonConvert.DeserializeObject<AgentOptions>(oldAgentOptionsString);
                await walletService.DeleteWalletAsync(oldAgentOptions.WalletConfiguration,
                    oldAgentOptions.WalletCredentials);
                // Add 1 sec delay to allow filesystem to catch up
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Wallet could not be deleted");
            }

            File.Delete(tempWalletPath);

            return agentoptions;
        }

        /// <inheritdoc />
        public async Task<List<long>> ListBackupsAsync(IAgentContext context)
        {
            var publicKey = await Did.KeyForLocalDidAsync(context.Wallet, InternalBackupDid);

            var listBackupsMessage = new ListBackupsAgentMessage()
            {
                BackupId = publicKey,
            };

            var connection = await GetMediatorConnectionAsync(context).ConfigureAwait(false);
            if (connection == null)
                throw new AriesFrameworkException(ErrorCode.RecordNotFound, "Couldn't locate a connection to mediator agent");

            var response = await messageService.SendReceiveAsync<ListBackupsResponseAgentMessage>(context, listBackupsMessage, connection).ConfigureAwait(false);
            return response.BackupList.ToList();
        }

        /// <inheritdoc />
        public async Task<AgentOptions> RestoreFromBackupAsync(IAgentContext context, string seed)
        {
            var backupAttachments = await RetrieveBackupAsync(context, seed);
            return await RestoreFromBackupAsync(context, seed, backupAttachments);
        }
    }
}
