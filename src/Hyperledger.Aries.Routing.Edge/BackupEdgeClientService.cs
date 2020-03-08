using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Decorators.Attachments;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Indy.CryptoApi;
using Multiformats.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Hyperledger.Aries.Routing.Edge
{
    public partial class EdgeClientService : IEdgeClientService
    {
        /// <inheritdoc />
        public async Task<DateTimeOffset> CreateBackupAsync(IAgentContext context, string seed)
        {
            if (seed.Length != 32)
            {
                throw new ArgumentException($"{nameof(seed)} should be 32 characters");
            }

            var publicKey = await Crypto.CreateKeyAsync(context.Wallet, new { seed }.ToJson());

            var path = Path.Combine(Path.GetTempPath(), seed);
            var json = new { path, seedPhrase = seed }.ToJson();

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

            var response = await messageService.SendReceiveAsync<StoreBackupResponseAgentMessage>(context.Wallet, backupMessage, connection).ConfigureAwait(false);

            File.Delete(path);

            return response.BackupTimestamp;
        }

        /// <inheritdoc />
        public async Task<List<Attachment>> RetrieveBackupAsync(IAgentContext context, string seed, DateTimeOffset offset = default)
        {
            var publicKey = await Crypto.CreateKeyAsync(context.Wallet, new { seed }.ToJson());

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
        public async Task<List<DateTimeOffset>> ListBackupsAsync(IAgentContext context, string seed)
        {
            var publicKey = await Crypto.CreateKeyAsync(context.Wallet, new { seed }.ToJson());

            var listBackupsMessage = new ListBackupsAgentMessage()
            {
                BackupId = publicKey,
            };

            var connection = await GetMediatorConnectionAsync(context).ConfigureAwait(false);
            if (connection == null)
                throw new AriesFrameworkException(ErrorCode.RecordNotFound, "Couldn't locate a connection to mediator agent");

            var response = await messageService.SendReceiveAsync<ListBackupsResponseAgentMessage>(context.Wallet, listBackupsMessage, connection).ConfigureAwait(false);
            return response.BackupList;
        }

        /// <inheritdoc />
        public Task RestoreFromBackupAsync(IAgentContext edgeContext, string seed, List<Attachment> backupData)
        {
            throw new NotImplementedException();
        }
    }
}
