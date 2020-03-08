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


        /// <summary>Stores the backup and all attachments.</summary>
        /// <param name="backupId">The backup identifier.</param>
        /// <param name="attachments">The attachments.</param>
        /// <returns>The datetime offset used for this backup</returns>
        /// <exception cref="ArgumentException">Invalid payload - attachments</exception>
        public Task<DateTimeOffset> StoreBackupAsync(string backupId, IEnumerable<Attachment> attachments)
        {
            if (attachments is null || !attachments.Any())
            {
                throw new ArgumentException("Invalid payload", nameof(attachments));
            }

            var nowUnix = DateTimeOffset.Now;
            var backupDir = Path.Combine(StorageDirectory, backupId, nowUnix.ToUnixTimeSeconds().ToString());

            if (!Directory.Exists(backupDir))
            {
                Directory.CreateDirectory(backupDir);
            }

            foreach (var attachment in attachments)
            {
                var payloadInBytes = attachment.Data.Base64.GetBytesFromBase64();
                File.WriteAllBytes(Path.Combine(backupDir, attachment.Id), payloadInBytes);
            }
            
            return Task.FromResult(nowUnix);
        }

        /// <summary>
        /// Retrieves the latest backup.
        /// </summary>
        /// <param name="backupId">The backup identifier.</param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">Wallet for key {backupId} was not found.</exception>
        public async Task<List<Attachment>> RetrieveBackupAsync(string backupId)
        {
            var backupDirectory = GetBackupPath(backupId);
            var backupList = await ListBackupsAsync(backupId);

            return await RetrieveBackupAsync(
                backupId: backupId,
                backupDate: DateTimeOffset.FromUnixTimeSeconds(long.Parse(backupList.OrderBy(x => x).Last())));
        }

        /// <summary>
        /// Lists the available backups for the given <c>backupId</c>.
        /// </summary>
        /// <param name="backupId">The backup identifier.</param>
        /// <returns></returns>
        public Task<IEnumerable<string>> ListBackupsAsync(string backupId)
        {
            return Task.FromResult(Directory.EnumerateDirectories(GetBackupPath(backupId)));
        }

        public Task<List<Attachment>> RetrieveBackupAsync(string backupId, DateTimeOffset backupDate)
        {
            var path = Path.Combine(GetBackupPath(backupId), backupDate.ToUnixTimeSeconds().ToString());

            return Task.FromResult(
                Directory.EnumerateFiles(path, "*", SearchOption.TopDirectoryOnly)
                .Select(x => new Attachment
                {
                    Id = Path.GetFileName(x),
                    Data = new AttachmentContent
                    {
                        Base64 = File.ReadAllBytes(x).ToBase64String()
                    }
                })
                .ToList());
        }
        
        private string GetBackupPath(string backupId) => Path.Combine(StorageDirectory, backupId);
    }
}